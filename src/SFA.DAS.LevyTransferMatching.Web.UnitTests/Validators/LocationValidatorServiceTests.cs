using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators
{
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

            // Assert
            CollectionAssert.Contains(multipleValidResults.Keys, 0);
            CollectionAssert.AreEqual(multipleValidResults[0], middletonSuggestions.Names);

            Assert.AreEqual(request.Locations[1], leicesterInformation.Name);

            CollectionAssert.Contains(multipleValidResults.Keys, 2);
            CollectionAssert.AreEqual(multipleValidResults[2], londonSuggestions.Names);
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

            // Assert
            CollectionAssert.Contains(result.Keys, 0);
            Assert.AreEqual("Check the spelling of your location", result[0]);

            CollectionAssert.Contains(result.Keys, 1);
            Assert.AreEqual("Duplicates of the same location are not allowed", result[1]);

            CollectionAssert.Contains(result.Keys, 2);
            Assert.AreEqual("Duplicates of the same location are not allowed", result[2]);
        }
    }
}