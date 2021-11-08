using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using static SFA.DAS.LevyTransferMatching.Web.Models.Applications.ApplicationViewModel;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    public class ApplicationPostRequestValidatorTests
    {
        private Fixture _fixture;
        private ApplicationPostRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _validator = new ApplicationPostRequestValidator();
        }

        [Test]
        public void ValidatorReturnsFalseWhenNoApprovalActionSelected()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, (ApprovalAction?)null)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenApprovalActionIsAcceptAndTruthfulInformationIsFalse()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, ApprovalAction.Accept)
                .With(x => x.TruthfulInformation, false)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenApprovalActionIsAcceptAndComplyWithRulesIsFalse()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, ApprovalAction.Accept)
                .With(x => x.ComplyWithRules, false)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsTrueWhenApprovalActionIsAcceptAndTruthfulInformationAndComplyWithRulesIsTrue()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, ApprovalAction.Accept)
                .With(x => x.TruthfulInformation, true)
                .With(x => x.ComplyWithRules, true)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsFalseWhenApprovalActionIsDeclineAndConfirmWithdrawalIsFalse()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, ApprovalAction.Decline)
                .With(x => x.IsDeclineConfirmed, false)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public void ValidatorReturnsTrueWhenApprovalActionIsDeclineAndConfirmWithdrawalIsTrue()
        {
            var request = _fixture
                .Build<ApplicationPostRequest>()
                .With(x => x.SelectedAction, ApprovalAction.Decline)
                .With(x => x.IsDeclineConfirmed, true)
                .Create();

            var actual = _validator.Validate(request);

            Assert.IsTrue(actual.IsValid);
        }
    }
}