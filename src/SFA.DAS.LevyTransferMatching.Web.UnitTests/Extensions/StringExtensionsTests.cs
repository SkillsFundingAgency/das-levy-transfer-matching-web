﻿using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
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
        public void ToReferenceDataDescriptionList_All_Selected()
        {
            // Arrange
            var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>();
            var selectedReferenceDataItemIds = referenceDataItems.Select(x => x.Id).ToArray();

            // Act
            var result = selectedReferenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

            // Assert
            Assert.AreEqual(result, "All");
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
    }
}