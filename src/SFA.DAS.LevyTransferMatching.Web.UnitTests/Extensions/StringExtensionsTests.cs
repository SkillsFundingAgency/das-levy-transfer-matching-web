using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private readonly Fixture _fixture;

        public StringExtensionsTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ToTagDescriptionList_All_Selected()
        {
            // Arrange
            var tags = _fixture.CreateMany<Tag>();
            var selectedTagIds = tags.Select(x => x.TagId).ToArray();

            // Act
            var result = selectedTagIds.ToTagDescriptionList(tags);

            // Assert
            Assert.AreEqual(result, "All");
        }

        [Test]
        public void ToTagDescriptionList_Some_Selected()
        {
            // Arrange
            var tags = _fixture.CreateMany<Tag>(6);
            var selectedTagIds = tags.OrderBy(x => Guid.NewGuid()).Take(3).Select(x => x.TagId).ToArray();

            // Act
            var result = selectedTagIds.ToTagDescriptionList(tags);

            // Assert
            var expectedList = string.Join(", ", tags
                .Where(x => selectedTagIds.Contains(x.TagId))
                .Select(x => x.Description)
                .ToArray());

            Assert.AreEqual(result, expectedList);
        }

        [Test]
        public void ToTagDescriptionList_One_Selected()
        {
            // Arrange
            var tags = _fixture.CreateMany<Tag>();
            var selectedTagIds = tags.OrderBy(x => Guid.NewGuid()).Take(1).Select(x => x.TagId).ToArray();

            // Act
            var result = selectedTagIds.ToTagDescriptionList(tags);

            // Assert
            var expectedTagDesc = tags.Where(x => selectedTagIds.Contains(x.TagId)).Select(x => x.Description).Single();

            Assert.AreEqual(result, expectedTagDesc);
        }
    }
}