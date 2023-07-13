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
    public class AutoApprovePostRequestValidatorTests
    {
        private AutoApprovePostRequestValidator organisationNamePostRequestValidator;

        [SetUp]
        public void SetUp()
        {
            organisationNamePostRequestValidator = new AutoApprovePostRequestValidator();
        }

      
        [TestCase(null)]        
        public void Validator_Returns_Expected_Errors_For_Invalid_OrganisationNameResponse(bool? autoApprove)
        {
            //Arrange
            AutoApprovePostRequest postRequest = new AutoApprovePostRequest()
            {
                AutoApproveFullMatches = autoApprove
            };

            //Act
            var result = organisationNamePostRequestValidator.TestValidate(postRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.AutoApproveFullMatches)
                .WithErrorMessage("You need to tell us if you want to approve 100% match or delay");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validator_Returns_No_Errors_For_Valid_OrganisationNameResponse(bool? autoApprove)
        {
            //Arrange
            AutoApprovePostRequest postRequest = new AutoApprovePostRequest()
            {
                AutoApproveFullMatches = autoApprove
            };

            //Act
            var result = organisationNamePostRequestValidator.TestValidate(postRequest);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.AutoApproveFullMatches);
        }

    }
}
