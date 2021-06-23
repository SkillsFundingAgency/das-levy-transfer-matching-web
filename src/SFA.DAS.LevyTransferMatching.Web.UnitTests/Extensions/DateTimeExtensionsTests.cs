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
            Assert.AreEqual("2008", taxYear);
        }

        [Test]
        public void ToTaxYear_Returns_Current_Year_When_April_Sixth_Short_Format()
        {
            // Arrange
            DateTime dateTime = new DateTime(1984, 4, 6);

            // Act
            string taxYear = dateTime.ToTaxYear("yy");

            // Assert
            Assert.AreEqual("84", taxYear);
        }
    }
}