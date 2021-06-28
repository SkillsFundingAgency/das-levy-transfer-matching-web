using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities
{
    [TestFixture]
    public class OpportunitiesPostRequestValidatorTests
    {
        private OpportunitiesPostRequestValidator _opportunitiesPostRequestValidator;

        [SetUp]
        public void Setup()
        {
            _opportunitiesPostRequestValidator = new OpportunitiesPostRequestValidator();
        }

        [TestCase(null)]
        public void Validator_Returns_Expected_Error_For_Null_HasConfirmed_Value(bool? hasConfirmed)
        {
            // Arrange
            OpportunitiesPostRequest opportunitiesPostRequest = new OpportunitiesPostRequest()
            {
                HasConfirmed = hasConfirmed,
            };

            // Act
            var result = _opportunitiesPostRequestValidator.TestValidate(opportunitiesPostRequest);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.HasConfirmed)
                .WithErrorMessage("You need to select Yes if you want to continue and apply for transfer funds");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validator_Returns_No_Errors_For_NonNull_Has_Confirmed_Value(bool? hasConfirmed)
        {
            // Arrange
            OpportunitiesPostRequest opportunitiesPostRequest = new OpportunitiesPostRequest()
            {
                HasConfirmed = hasConfirmed,
            };

            // Act
            var result = _opportunitiesPostRequestValidator.TestValidate(opportunitiesPostRequest);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.HasConfirmed);
        }
    }
}