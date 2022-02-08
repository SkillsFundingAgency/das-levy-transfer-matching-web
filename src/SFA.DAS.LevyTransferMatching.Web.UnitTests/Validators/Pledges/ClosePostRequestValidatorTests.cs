using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Pledges
{
    [TestFixture]
    public class CloseApprovalOptionsPostRequestValidatorTests
    {
        private ClosePostRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ClosePostRequestValidator();
        }

        [Test]
        public void Validator_Returns_Error_When_HasConfirmed_Is_Null()
        {
            var request = new ClosePostRequest
            {
                HasConfirmed = null
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.HasConfirmed)
                .WithErrorMessage("You need to select Yes if you want to continue and close this pledge");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validator_Returns_No_Error_When_HasConfirmed_Has_Value(bool hasConfirmed)
        {
            var request = new ClosePostRequest
            {
                HasConfirmed = hasConfirmed
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.HasConfirmed);
        }
    }
}
