using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void ToReferenceDataDescriptionList_All_Selected()
    {
        // Arrange
        var referenceDataItems = _fixture.CreateMany<ReferenceDataItem>();
        var selectedReferenceDataItemIds = referenceDataItems.Select(x => x.Id).ToArray();

        // Act
        var result = selectedReferenceDataItemIds.ToReferenceDataDescriptionList(referenceDataItems);

        // Assert
        result.Should().Be("All");
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

        expectedList.Should().Be(result);
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

        expectedTagDesc.Should().Be(result);
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
        result.Should().Be("All");
    }

    [Test]
    public void IsComplete_Returns_False_When_Text_Is_NotCompletedString()
    {
        "-".IsComplete().Should().BeFalse();
    }

    [Test]
    public void IsComplete_Returns_False_When_Text_Is_Null()
    {
        string str = null;

        str.IsComplete().Should().BeFalse();
    }

    [Test]
    public void IsComplete_Returns_False_When_Text_Is_Empty()
    {
        string.Empty.IsComplete().Should().BeFalse();
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

        locationsString.Should().Be(expected);
    }

    [Test]
    public void ToApplicationLocationsString_Returns_Correct_String_With_No_Matches()
    {
        var locations = new List<string>();
        const string additionalLocation = "Additional location test";

        var locationsString = locations.ToApplicationLocationsString(", ", additionalLocation);

        locationsString.Should().Be(additionalLocation);
    }
}