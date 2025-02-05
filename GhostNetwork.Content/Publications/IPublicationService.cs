﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.EventBus;

namespace GhostNetwork.Content.Publications
{
    public interface IPublicationService
    {
        Task<Publication> GetByIdAsync(string id);

        Task<IReadOnlyCollection<Publication>> SearchAsync(IEnumerable<string> tags, Ordering order, Pagination pagination);

        Task<(DomainResult, string)> CreateAsync(string text, UserInfo author);

        Task<DomainResult> UpdateAsync(string id, string text);

        Task DeleteAsync(string id);

        Task<IReadOnlyCollection<Publication>> SearchByAuthorAsync(Guid authorId, Ordering order, Pagination pagination);
    }

    public class PublicationService : IPublicationService
    {
        private readonly IValidator<Publication> validator;
        private readonly IPublicationsStorage publicationStorage;
        private readonly IHashTagsFetcher hashTagsFetcher;
        private readonly IEventBus eventBus;

        public PublicationService(
            IValidator<Publication> validator,
            IPublicationsStorage publicationStorage,
            IHashTagsFetcher hashTagsFetcher,
            IEventBus eventBus)
        {
            this.validator = validator;
            this.publicationStorage = publicationStorage;
            this.hashTagsFetcher = hashTagsFetcher;
            this.eventBus = eventBus;
        }

        public Task<Publication> GetByIdAsync(string id)
        {
            return publicationStorage.FindOneByIdAsync(id);
        }

        public Task<IReadOnlyCollection<Publication>> SearchAsync(IEnumerable<string> tags, Ordering order, Pagination pagination)
        {
            return publicationStorage.FindManyAsync(tags, order, pagination);
        }

        public async Task<(DomainResult, string)> CreateAsync(string text, UserInfo author)
        {
            var publication = Publication.New(text, author, hashTagsFetcher.Fetch);
            var result = await validator.ValidateAsync(publication);

            if (!result.Successed)
            {
                return (result, null);
            }

            var id = await publicationStorage.InsertOneAsync(publication);

            await eventBus.PublishAsync(new CreatedEvent(id, publication.Content, author));

            return (result, id);
        }

        public async Task<DomainResult> UpdateAsync(string id, string text)
        {
            var publication = await publicationStorage.FindOneByIdAsync(id);

            publication.Update(text, hashTagsFetcher.Fetch);
            var result = await validator.ValidateAsync(publication);

            if (!result.Successed)
            {
                return result;
            }

            await publicationStorage.UpdateOneAsync(publication);

            await eventBus.PublishAsync(new UpdatedEvent(
                publication.Id,
                publication.Content,
                publication.Author));

            return DomainResult.Success();
        }

        public async Task DeleteAsync(string id)
        {
            var publication = await publicationStorage.FindOneByIdAsync(id);
            await publicationStorage.DeleteOneAsync(id);
            await eventBus.PublishAsync(new DeletedEvent(publication.Id, publication.Author));
        }

        public Task<IReadOnlyCollection<Publication>> SearchByAuthorAsync(
            Guid authorId,
            Ordering order,
            Pagination pagination)
        {
            return publicationStorage.FindManyByAuthorAsync(authorId, order, pagination);
        }
    }
}
