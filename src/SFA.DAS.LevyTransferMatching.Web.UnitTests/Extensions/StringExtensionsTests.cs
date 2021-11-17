using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System;
using System.Collections.Generic;
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
        public void ToReferenceDataDescriptionList_All_Selected()
        {
            // Arrange
            var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>();
            var selectedReferenceDataItemIds = referenceDataItems.Select(x => x.Id).ToArray();

            // Act
            var result = selectedReferenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

            // Assert
            Assert.AreEqual("All", result);
        }

        [Test]
        public void ToReferenceDataDescriptionList_Some_Selected()
        {
            // Arrange
            var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>(6);
            var selectedReferenceDataItemIds = referenceDataItems.OrderBy(x => Guid.NewGuid()).Take(3).Select(x => x.Id).ToArray();

            // Act
            var result = selectedReferenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

            // Assert
            var expectedList = string.Join(", ", referenceDataItems
                .Where(x => selectedReferenceDataItemIds.Contains(x.Id))
                .Select(x => x.Description)
                .ToArray());

            Assert.AreEqual(result, expectedList);
        }

        [Test]
        public void ToReferenceDataDescriptionList_One_Selected()
        {
            // Arrange
            var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>();
            var referenceDataItemIds = referenceDataItems.OrderBy(x => Guid.NewGuid()).Take(1).Select(x => x.Id).ToArray();

            // Act
            var result = referenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

            // Assert
            var expectedTagDesc = referenceDataItems.Where(x => referenceDataItemIds.Contains(x.Id)).Select(x => x.Description).Single();

            Assert.AreEqual(result, expectedTagDesc);
        }
        
        [Test]
        public void ToReferenceDataDescriptionList_None_Selected()
        {
            // Arrange
            var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>();
            var referenceDataItemIds = Array.Empty<string>();

            // Act
            var result = referenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

            // Assert
            Assert.AreEqual("All", result);
        }

        [Test]
        public void IsComplete_Returns_False_When_Text_Is_NotCompletedString()
        {
            Assert.IsFalse("-".IsComplete());
        }

        [Test]
        public void IsComplete_Returns_False_When_Text_Is_Null()
        {
            string str = null;

            Assert.IsFalse(str.IsComplete());
        }

        [Test]
        public void IsComplete_Returns_False_When_Text_Is_Empty()
        {
            Assert.IsFalse(string.Empty.IsComplete());
        }

        [TestCase("AdditionalLocation", "Manchester, Coventry, AdditionalLocation")]
        [TestCase("", "Manchester, Coventry")]
        public void ToApplicationLocationsString_Returns_Correct_String_With_Matches(string additionalLocation, string expected)
        {
            var locations = new List<string>
            {
                "Manchester",
                "Coventry"
            };

            var locationsString = locations.ToApplicationLocationsString(", ", additionalLocation);

            Assert.AreEqual(expected, locationsString);
        }

        [Test]
        public void ToApplicationLocationsString_Returns_Correct_String_With_No_Matches()
        {
            var locations = new List<string>();
            var additionalLocation = "Additional location test";

            var locationsString = locations.ToApplicationLocationsString(", ", additionalLocation);

            Assert.AreEqual(additionalLocation, locationsString);
        }
    }
}