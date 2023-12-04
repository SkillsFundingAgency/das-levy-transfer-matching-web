using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using static SFA.DAS.LevyTransferMatching.Web.Models.Applications.ApplicationViewModel;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications;

[TestFixture]
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

        Assert.That(actual.IsValid, Is.False);
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

        Assert.That(actual.IsValid, Is.False);
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

        Assert.That(actual.IsValid, Is.False);
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
        Assert.That(actual.IsValid, Is.True);
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

        Assert.That(actual.IsValid, Is.False);
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

        Assert.That(actual.IsValid, Is.True);
    }

    [Test]
    public void ValidatorReturnsFalseWhenWithdrawalNotConfirmed()
    {
        var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: ApprovalAction.Withdraw));
        Assert.That(actual.IsValid, Is.False);
    }

    [Test]
    public void ValidatorReturnsFalseWhenNoWithdrawalActionSelected()
    {
        var actual = _validator.Validate(CreateApplicationStatusPostRequest(approvalAction: null, canWithdraw: true));
        Assert.That(actual.IsValid, Is.False);
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