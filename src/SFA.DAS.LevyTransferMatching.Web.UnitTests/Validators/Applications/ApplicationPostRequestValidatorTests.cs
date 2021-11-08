using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using static SFA.DAS.LevyTransferMatching.Web.Models.Applications.ApplicationViewModel;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    [TestFixture]
    public class ApplicationPostRequestValidatorTests
    {
        private ApplicationPostRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ApplicationPostRequestValidator();
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
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(truthfulInformation: true, complyWithRules: true, approvalAction: null, canAcceptFunding: true));

            Assert.AreEqual(false, actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenNoWithdrawalActionSelected()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: null, canWithdraw: true));

            Assert.AreEqual(false, actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenWithdrawalNotConfirmed()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: ApprovalAction.Withdraw));

            Assert.AreEqual(false, actual.IsValid);
        }

        private ApplicationPostRequest CreateApplicationStatusPostRequest(bool truthfulInformation = false, bool complyWithRules = false, ApprovalAction? approvalAction = ApprovalAction.Decline, bool canAcceptFunding = false, bool canWithdraw = false, bool isWithdrawalConfirmed = false) =>
            new ApplicationPostRequest()
            {
                EncodedAccountId = "HGVVMY",
                AccountId = 1,
                ApplicationId = 1,
                TruthfulInformation = truthfulInformation,
                ComplyWithRules = complyWithRules,
                EncodedApplicationId = "YTVWM6",
                SelectedAction = approvalAction,
                CanAcceptFunding = canAcceptFunding,
                CanWithdraw = canWithdraw,
                IsWithdrawalConfirmed = isWithdrawalConfirmed
            };
    }
}