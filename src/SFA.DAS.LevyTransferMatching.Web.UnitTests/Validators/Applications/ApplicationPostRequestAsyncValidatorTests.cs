using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    public class ApplicationPostRequestAsyncValidatorTests
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
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: Web.Models.Pledges.ApplicationPostRequest.ApprovalAction.Approve));

            Assert.AreEqual(false, actual.IsValid);
        }


        [Test]
        public void ValidatorReturnsFalseWhenUserHasNotAcceptedFunding()
        {
            var actual = _validator.Validate(CreateApplicationStatusPostRequest(truthfulInformation: true, complyWithRules: true));

            Assert.AreEqual(false, actual.IsValid);
        }

        private ApplicationPostRequest CreateApplicationStatusPostRequest(bool truthfulInformation = false, bool complyWithRules = false, Web.Models.Pledges.ApplicationPostRequest.ApprovalAction approvalAction = Web.Models.Pledges.ApplicationPostRequest.ApprovalAction.Reject) =>
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
