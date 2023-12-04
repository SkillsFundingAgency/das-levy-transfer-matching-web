using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ToTaxYear_Returns_Previous_Year_When_April_Fifth_Full_Format()
        {
            // Arrange
            DateTime dateTime = new DateTime(2009, 4, 5);

            // Act
            string taxYear = dateTime.ToTaxYear("yyyy");

            // Assert
            Assert.That(taxYear, Is.EqualTo("2008"));
        }

        [Test]
        public void ToTaxYear_Returns_Current_Year_When_April_Sixth_Short_Format()
        {
            // Arrange
            DateTime dateTime = new DateTime(1984, 4, 6);

            // Act
            string taxYear = dateTime.ToTaxYear("yy");

            // Assert
            Assert.That(taxYear, Is.EqualTo("84"));
        }

        [Test]
        public void ToTaxYearDescription_Returns_Previous_Year_And_This_Year_When_April_Fifth()
        {
            // Arrange
            DateTime dateTime = new DateTime(1992, 4, 5);

            // Act
            string taxYear = dateTime.ToTaxYearDescription();

            // Assert
            Assert.That(taxYear, Is.EqualTo("1991/92"));
        }

        [Test]
        public void ToTaxYearDescription_Returns_Current_Year_And_Next_Year_When_April_Sixth()
        {
            // Arrange
            DateTime dateTime = new DateTime(2015, 4, 6);

            // Act
            string taxYear = dateTime.ToTaxYearDescription();

            // Assert
            Assert.That(taxYear, Is.EqualTo("2015/16"));
        }
    }
}