using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    public class ApplicationStatusPostRequestAsyncValidatorTests
    {
        private ApplicationStatusPostRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ApplicationStatusPostRequestValidator();
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
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: ApplicationPostRequest.ApprovalAction.Approve));

            Assert.AreEqual(false, actual.IsValid);
        }


        [Test]
        public void ValidatorReturnsFalseWhenUserHasNotAcceptedFunding()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(truthfulInformation: true, complyWithRules: true));

            Assert.AreEqual(false, actual.IsValid);
        }

        private ApplicationStatusPostRequest CreateApplicationStatusPostRequest(bool truthfulInformation = false, bool complyWithRules = false, ApplicationPostRequest.ApprovalAction approvalAction = ApplicationPostRequest.ApprovalAction.Reject) =>
            new ApplicationStatusPostRequest()
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
