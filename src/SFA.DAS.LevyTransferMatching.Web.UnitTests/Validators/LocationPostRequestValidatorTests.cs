using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators
{
    [TestFixture]
    public class LocationPostRequestValidatorTests
    {
        private LocationPostRequestValidator locationPostRequestValidator;

        [SetUp]
        public void SetUp()
        {
            locationPostRequestValidator = new LocationPostRequestValidator();
        }

        [Test]
        public void Validator_Returns_No_Errors_For_No_Duplicate_Locations()
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

            //Act
            var result = locationPostRequestValidator.TestValidate(locationPostRequest);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Locations);
        }

        [Test]
        public void Validator_Returns_Expected_Errors_For_Duplicate_Locations()
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

            //Act
            var result = locationPostRequestValidator.TestValidate(locationPostRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Locations);
        }
    }
}
