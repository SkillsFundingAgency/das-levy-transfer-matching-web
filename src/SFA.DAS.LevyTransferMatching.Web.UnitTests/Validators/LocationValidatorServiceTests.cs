using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators;

[TestFixture]
public class LocationValidatorServiceTests
{
    private Mock<ILocationService> _locationService;
    private LocationValidatorService _locationValidatorService;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _locationService = new Mock<ILocationService>();
        _fixture = new Fixture();

        _locationValidatorService = new LocationValidatorService(_locationService.Object);
    }

    [Test]
    public async Task ValidateLocations_One_Auto_Update_Two_With_Multiple_Valid_Results()
    {
        // Arrange
        var enteredLocations = new List<string>()
        {
            "Middleton",
            "Leicester",
            "London",
        };

        var middletonSuggestions = _fixture.Create<LocationsDto>();
        var leicesterSuggestions = _fixture
            .Build<LocationsDto>()
            .With(x => x.Names, _fixture.CreateMany<string>(1))
            .Create();
        var londonSuggestions = _fixture.Create<LocationsDto>();

        var request = _fixture
            .Build<LocationPostRequest>()
            .With(x => x.Locations, enteredLocations)
            .Create();

        var multipleValidResults = new Dictionary<int, IEnumerable<string>>();

        _locationService
            .Setup(x => x.GetLocations(It.Is<string>(y => y == "Middleton")))
            .ReturnsAsync(middletonSuggestions);
            
        _locationService
            .Setup(x => x.GetLocations(It.Is<string>(y => y == "Leicester")))
            .ReturnsAsync(leicesterSuggestions);

        var leicesterInformation = _fixture.Create<LocationInformationDto>();

        _locationService
            .Setup(x => x.GetLocationInformation(It.Is<string>(y => y == "Leicester")))
            .ReturnsAsync(leicesterInformation);

        _locationService
            .Setup(x => x.GetLocations(It.Is<string>(y => y == "London")))
            .ReturnsAsync(londonSuggestions);

        // Act
        var result = await _locationValidatorService.ValidateLocations(request, multipleValidResults);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(multipleValidResults.Keys, Has.Member(0));
            Assert.That(middletonSuggestions.Names, Is.EqualTo(multipleValidResults[0]).AsCollection);
            Assert.That(leicesterInformation.Name, Is.EqualTo(request.Locations[1]));
            Assert.That(multipleValidResults.Keys, Has.Member(2));
            Assert.That(londonSuggestions.Names, Is.EqualTo(multipleValidResults[2]).AsCollection);
        });
    }

    [Test]
    public async Task ValidateLocations_One_Typo_One_Duplicate()
    {
        // Arrange
        var enteredLocations = new List<string>()
        {
            "Macester",
            "Leicester",
            "Leicester",
        };

        var macesterSuggestions = _fixture
            .Build<LocationsDto>()
            .With(x => x.Names, _fixture.CreateMany<string>(0))
            .Create();
        var leicesterSuggestions = _fixture
            .Build<LocationsDto>()
            .With(x => x.Names, _fixture.CreateMany<string>(1))
            .Create();

        var request = _fixture
            .Build<LocationPostRequest>()
            .With(x => x.Locations, enteredLocations)
            .Create();

        var multipleValidResults = new Dictionary<int, IEnumerable<string>>();

        _locationService
            .Setup(x => x.GetLocations(It.Is<string>(y => y == "Macester")))
            .ReturnsAsync(macesterSuggestions);
            
        _locationService
            .Setup(x => x.GetLocations(It.Is<string>(y => y == "Leicester")))
            .ReturnsAsync(leicesterSuggestions);

        var leicesterInformation = _fixture.Create<LocationInformationDto>();

        _locationService
            .Setup(x => x.GetLocationInformation(It.Is<string>(y => y == "Leicester")))
            .ReturnsAsync(leicesterInformation);

        // Act
        var result = await _locationValidatorService.ValidateLocations(request, multipleValidResults);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Keys, Has.Member(0));
            Assert.That(result[0], Is.EqualTo("Check the spelling of your location"));
            
            Assert.That(result.Keys, Has.Member(1));
            Assert.That(result[1], Is.EqualTo("Duplicates of the same location are not allowed"));

            Assert.That(result.Keys, Has.Member(2));
            Assert.That(result[2], Is.EqualTo("Duplicates of the same location are not allowed"));
        });
    }
}