using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

[TestFixture]
public class DateTimeExtensionsTests
{
    [Test]
    public void ToTaxYear_Returns_Previous_Year_When_April_Fifth_Full_Format()
    {
        // Arrange
        var dateTime = new DateTime(2009, 4, 5);

        // Act
        var taxYear = dateTime.ToTaxYear("yyyy");

        // Assert
        Assert.That(taxYear, Is.EqualTo("2008"));
    }

    [Test]
    public void ToTaxYear_Returns_Current_Year_When_April_Sixth_Short_Format()
    {
        // Arrange
        var dateTime = new DateTime(1984, 4, 6);

        // Act
        var taxYear = dateTime.ToTaxYear("yy");

        // Assert
        Assert.That(taxYear, Is.EqualTo("84"));
    }

    [Test]
    public void ToTaxYearDescription_Returns_Previous_Year_And_This_Year_When_April_Fifth()
    {
        // Arrange
        var dateTime = new DateTime(1992, 4, 5);

        // Act
        var taxYear = dateTime.ToTaxYearDescription();

        // Assert
        Assert.That(taxYear, Is.EqualTo("1991/92"));
    }

    [Test]
    public void ToTaxYearDescription_Returns_Current_Year_And_Next_Year_When_April_Sixth()
    {
        // Arrange
        var dateTime = new DateTime(2015, 4, 6);

        // Act
        var taxYear = dateTime.ToTaxYearDescription();

        // Assert
        Assert.That(taxYear, Is.EqualTo("2015/16"));
    }
}