using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators
{
    [TestFixture]
    public class ValidatorServiceTests
    {
        private ILocationValidatorService validatorService;
        private Mock<ILocationService> locationService;

        [SetUp]
        public void SetUp()
        {
            locationService = new Mock<ILocationService>();
        }

        [Test]
        public async Task ValidateLocations_Produces_No_Errors_With_Valid_Locations()
        {
            //Arrange
            var locationPostRequest = new LocationPostRequest
            {
                Locations = new List<string>
                {
                    "Manchester",
                    "Warwick",
                    "Stoke"
                }
            };

            foreach (var location in locationPostRequest.Locations)
            {
                locationService.Setup(x => x.GetLocationInformation(location)).Returns(Task.FromResult(new LocationInformationDto { Name = location }));
            };

            validatorService = new LocationValidatorService(locationService.Object);

            //Act
            var errorsResult = await validatorService.ValidateLocations(locationPostRequest);

            //Assert
            Assert.That(!errorsResult.Any());
        }

        [Test]
        public async Task ValidateLocations_Produces_Errors_With_Duplicate_Locations()
        {
            //Arrange
            var locationPostRequest = new LocationPostRequest
            {
                Locations = new List<string>
                {
                    "Manchester",
                    "Warwick",
                    "Manchester"
                }
            };

            foreach (var location in locationPostRequest.Locations)
            {
                locationService.Setup(x => x.GetLocationInformation(location)).Returns(Task.FromResult(new LocationInformationDto { Name = location }));
            };

            validatorService = new LocationValidatorService(locationService.Object);

            //Act
            var errorsResult = await validatorService.ValidateLocations(locationPostRequest);

            //Assert
            Assert.That(errorsResult.Any());
        }

        [Test]
        public async Task ValidateLocations_Produces_Errors_With_NotFound_Locations()
        {
            //Arrange
            var locationPostRequest = new LocationPostRequest
            {
                Locations = new List<string>
                {
                    "Manchester",
                    "IncorrectLocation"
                }
            };

            locationService.Setup(x => x.GetLocationInformation("Manchester")).Returns(Task.FromResult(new LocationInformationDto { Name = "Manchester" }));
            locationService.Setup(x => x.GetLocationInformation("IncorrectLocation")).Returns(Task.FromResult(new LocationInformationDto { Name = null }));

            validatorService = new LocationValidatorService(locationService.Object);

            //Act
            var errorsResult = await validatorService.ValidateLocations(locationPostRequest);

            //Assert
            Assert.That(errorsResult.Any());
        }
    }
}
