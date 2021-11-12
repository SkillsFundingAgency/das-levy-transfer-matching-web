using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using static SFA.DAS.LevyTransferMatching.Web.Models.Applications.ApplicationViewModel;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    public class ApplicationPostRequestValidatorTests
    {
        private ApplicationPostRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ApplicationPostRequestValidator();
        }

        [Test]
        public void ValidatorReturnsFalseWhenSelectedActionIsSetToDenyAndWhenUserHasNotAcceptedTermsAndConditions()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest());

            Assert.AreEqual(false, actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenUserHasNotAcceptedTermsAndConditions()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: ApprovalAction.Accept));

            Assert.AreEqual(false, actual.IsValid);
        }


        [Test]
        public void ValidatorReturnsFalseWhenUserHasNotAcceptedFunding()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(truthfulInformation: true, complyWithRules: true, approvalAction: null));

            Assert.AreEqual(false, actual.IsValid);
        }

        private ApplicationPostRequest CreateApplicationStatusPostRequest(bool truthfulInformation = false, bool complyWithRules = false, ApprovalAction? approvalAction = ApprovalAction.Decline) =>
            new ApplicationPostRequest()
            {
                EncodedAccountId = "HGVVMY",
                AccountId = 1,
                ApplicationId = 1,
                TruthfulInformation = truthfulInformation,
                ComplyWithRules = complyWithRules,
                EncodedApplicationId = "YTVWM6",
                SelectedAction = approvalAction
            };
    }
}