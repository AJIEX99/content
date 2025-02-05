﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Comments;
using GhostNetwork.Content.Publications;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest
{
    [TestFixture]
    public class MaxLengthValidatorTests
    {
        [Test]
        public async Task Publication_Content_Length_Longer_Than_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hello_Hello";

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now);

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Shorter_Than_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hi";

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now);

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Publication_Content_Length_Equal_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hello";

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                null,
                DateTimeOffset.Now,
                DateTimeOffset.Now);

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Longer_Than_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hello_Hello";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Shorter_Than_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hi";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task Comment_Content_Length_Equal_MaxLength()
        {
            // Setup
            var validator = new MaxLengthValidator(5);
            var content = "Hello";

            var comment = new Comment(string.Empty, content, DateTimeOffset.Now, string.Empty, null, null);

            // Act
            var result = await validator.ValidateAsync(comment);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
