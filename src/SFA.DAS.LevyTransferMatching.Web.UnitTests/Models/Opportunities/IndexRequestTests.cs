using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Models.Opportunities;

public class IndexRequestTests
{
    private readonly Fixture _fixture = new();  

    [Test]
    public void GetSectorsList_ShouldReturnCommaSeparatedSectors_WhenCommaSeparatedSectorsIsNotNullOrWhiteSpace()
    {
        // Arrange
        var request = _fixture.Create<IndexRequest>();
        request.CommaSeparatedSectors = "Sector1,Sector2";
        request.Sectors = null;

        // Act
        var result = request.GetSectorsList();

        // Assert
        result.Should().BeEquivalentTo(new List<string> { "Sector1", "Sector2" });
    }

    [Test]
    public void GetSectorsList_ShouldReturnSectors_WhenCommaSeparatedSectorsIsNullOrWhiteSpace()
    {
        // Arrange
        var request = _fixture.Create<IndexRequest>();
        request.CommaSeparatedSectors = null;
        var sectors = new List<string> { "Sector1", "Sector2" };
        request.Sectors = sectors;

        // Act
        var result = request.GetSectorsList();

        // Assert
        result.Should().BeEquivalentTo(sectors);
    }

    [Test]
    public void GetSectorsList_ShouldReturnEmptyList_WhenCommaSeparatedSectorsAndSectorsAreNullOrWhiteSpace()
    {
        // Arrange
        var request = _fixture.Create<IndexRequest>();
        request.CommaSeparatedSectors = null;
        request.Sectors = null;

        // Act
        var result = request.GetSectorsList();

        // Assert
        result.Should().BeNull();
    }
}
