﻿using FluentAssertions.Execution;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Services;
using ApplicationRequest = SFA.DAS.LevyTransferMatching.Web.Models.Pledges.ApplicationRequest;
using GetApplicationsResponse = SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types.GetApplicationsResponse;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class PledgeOrchestratorTests
{
    private PledgeOrchestrator _orchestrator;
    private Fixture _fixture;
    private Mock<IPledgeService> _pledgeService;
    private Mock<IEncodingService> _encodingService;
    private Mock<IUserService> _userService;
    private Mock<IDateTimeService> _dateTimeService;
    private Mock<ICsvHelperService> _csvService;
    private Infrastructure.Configuration.FeatureToggles _featureToggles;
    private List<ReferenceDataItem> _sectors;
    private List<ReferenceDataItem> _levels;
    private List<ReferenceDataItem> _jobRoles;
    private GetAmountResponse _amountResponse;
    private GetSectorResponse _sectorResponse;
    private GetJobRoleResponse _jobRoleResponse;
    private GetLevelResponse _levelResponse;
    private GetPledgesResponse _pledgesResponse;
    private GetApplicationApprovedResponse _applicationApprovedResponse;
    private string _encodedAccountId;
    private readonly long _accountId = 1;
    private readonly int _pledgeId = 1;
    private string _encodedPledgeId;
    private string _encodedApplicationId;
    private readonly int _applicationId = 1;
    private string _userId;
    private string _userDisplayName;
    private decimal _startingTransferAllowance = 20000;
    private int _page = 1;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _encodedAccountId = _fixture.Create<string>();
        _pledgeService = new Mock<IPledgeService>();
        _encodingService = new Mock<IEncodingService>();
        _userService = new Mock<IUserService>();
        _dateTimeService = new Mock<IDateTimeService>();
        _csvService = new Mock<ICsvHelperService>();
        _dateTimeService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _featureToggles = new Infrastructure.Configuration.FeatureToggles();

        _sectors = _fixture.Create<List<ReferenceDataItem>>();
        _levels = _fixture.Create<List<ReferenceDataItem>>();
        _jobRoles = _fixture.Create<List<ReferenceDataItem>>();

        _amountResponse = _fixture.Create<GetAmountResponse>();
        _sectorResponse = new GetSectorResponse { Sectors = _sectors };
        _levelResponse = new GetLevelResponse { Levels = _levels };
        _jobRoleResponse = new GetJobRoleResponse { JobRoles = _jobRoles, Sectors = _sectors };
        _pledgesResponse = _fixture.Create<GetPledgesResponse>();
        _pledgesResponse.StartingTransferAllowance = _startingTransferAllowance;

        _applicationApprovedResponse = _fixture.Create<GetApplicationApprovedResponse>();

        _encodedPledgeId = _fixture.Create<string>();
        _encodedApplicationId = _fixture.Create<string>();

        _pledgeService.Setup(x => x.GetPledges(_accountId, _page, PledgesRequest.DefaultPageSize)).ReturnsAsync(_pledgesResponse);
        _pledgeService.Setup(x => x.GetCreate(_accountId)).ReturnsAsync(() => new GetCreateResponse
        {
            Sectors = _sectors,
            JobRoles = _jobRoles,
            Levels = _levels
        });

        _pledgeService.Setup(x => x.GetAmount(_encodedAccountId)).ReturnsAsync(_amountResponse);
        _pledgeService.Setup(x => x.GetSector(_accountId)).ReturnsAsync(_sectorResponse);
        _pledgeService.Setup(x => x.GetJobRole(_accountId)).ReturnsAsync(_jobRoleResponse);
        _pledgeService.Setup(x => x.GetLevel(_accountId)).ReturnsAsync(_levelResponse);
        _pledgeService.Setup(x => x.GetApplicationApproved(_accountId, _pledgeId, _applicationId))
            .ReturnsAsync(_applicationApprovedResponse);

        _userId = _fixture.Create<string>();
        _userDisplayName = _fixture.Create<string>();
        _userService.Setup(x => x.IsUserChangeAuthorized(_encodedAccountId)).Returns(true);
        _userService.Setup(x => x.GetUserId()).Returns(_userId);
        _userService.Setup(x => x.GetUserDisplayName()).Returns(_userDisplayName);
        _userService.Setup(x => x.IsOwnerOrTransactor(_encodedAccountId)).Returns(true);

        _orchestrator = new PledgeOrchestrator(_pledgeService.Object, _encodingService.Object, _userService.Object,
            _featureToggles,
            _dateTimeService.Object, _csvService.Object);
    }

    [Test]
    public async Task GetPledgesViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetPledgesViewModel_RenderCreatePledgeButton_Is_True_When_Authorized()
    {
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.RenderCreatePledgeButton.Should().BeTrue();
    }

    [Test]
    public async Task GetPledgesViewModel_Pledges_Is_Populated()
    {
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Pledges.Should().NotBeNull();
    }

    [Test]
    public async Task GetPledgesViewModel_Should_Set_HasMinimumTransferFunds_To_True_When_Conditions_Met()
    {
        // Arrange     
        var pledges = _fixture.Build<GetPledgesResponse>()
            .With(x => x.CurrentYearEstimatedCommittedSpend, 4000)
            .With(x => x.StartingTransferAllowance, _startingTransferAllowance)
            .Create();

        // Act
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });

        // Assert
        result.HasMinimumTransferFunds.Should().BeTrue();
    }

    [Test]
    public async Task GetPledgesViewModel_Should_Set_HasMinimumTransferFunds_To_False_When_Conditions_Not_Met()
    {
        // Arrange       
        var pledges = _fixture.Build<GetPledgesResponse>()
            .With(x => x.CurrentYearEstimatedCommittedSpend, 44000)
            .With(x => x.StartingTransferAllowance, _startingTransferAllowance)
            .Create();

        _pledgeService.Setup(x => x.GetPledges(_accountId, _page, 50))
            .ReturnsAsync(pledges);

        // Act
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });

        // Assert
        result.HasMinimumTransferFunds.Should().BeFalse();
    }

    [Test]
    public async Task GetApplications_Returns_Valid_ViewModel()
    {
        var response = new GetApplicationsResponse
        {
            Items =
            [
                new()
                {
                    Id = 0,
                    StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                    Status = ApplicationStatus.Pending
                }
            ]
        };

        _pledgeService.Setup(x => x.GetApplications(0, 0, 1, null, null, ApplicationsRequest.PageSize)).ReturnsAsync(response);
        _encodingService.Setup(x => x.Encode(0, EncodingType.PledgeApplicationId)).Returns("123");

        var result = await _orchestrator.GetApplications(new ApplicationsRequest
            { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });

        result.EncodedAccountId.Should().Be(_encodedAccountId);
        result.EncodedPledgeId.Should().Be(_encodedPledgeId);
        result.Applications.Should().AllSatisfy(application =>
        {
            application.EncodedApplicationId.Should().Be("123");
            application.Status.Should().Be(ApplicationStatus.Pending);
        });
    }


    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(2, true)]
    public async Task GetApplications_With_Pending_Status_Will_Renders_Reject_Button(int numberOfPendingApplications,
        bool expectedRenderButton)
    {
        var response = new GetApplicationsResponse
        {
            Items = Enumerable.Range(0, numberOfPendingApplications)
                .Select(_ => _fixture.Build<GetApplicationsResponse.Application>()
                    .With(p => p.Id, 0)
                    .With(p => p.Status, ApplicationStatus.Pending)
                    .With(p => p.StartDate, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
                    .Create()).ToList()
        };

        _pledgeService.Setup(x => x.GetApplications(0, 0, 1, null, null, ApplicationsRequest.PageSize)).ReturnsAsync(response);
        _encodingService.Setup(x => x.Encode(0, EncodingType.PledgeApplicationId)).Returns("123");

        var result = await _orchestrator.GetApplications(new ApplicationsRequest
        { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });
        result.RenderRejectButton.Should().Be(expectedRenderButton);
    }

    [TestCase(ApplicationStatus.Withdrawn)]
    [TestCase(ApplicationStatus.Approved)]
    [TestCase(ApplicationStatus.Accepted)]
    [TestCase(ApplicationStatus.Declined)]
    public async Task GetApplications_Without_Pending_Status_Will_Not_Render_Reject_Button(ApplicationStatus status)
    {
        var response = new GetApplicationsResponse
        {
            Items = Enumerable.Range(0, 1)
                .Select(_ => _fixture.Build<GetApplicationsResponse.Application>()
                    .With(p => p.Id, 0)
                    .With(p => p.Status, status)
                    .With(p => p.StartDate, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
                    .Create()).ToList()
        };

        _pledgeService.Setup(x => x.GetApplications(0, 0, 1, null, null, ApplicationsRequest.PageSize)).ReturnsAsync(response);
        _encodingService.Setup(x => x.Encode(0, EncodingType.PledgeApplicationId)).Returns("123");

        var result = await _orchestrator.GetApplications(new ApplicationsRequest
        { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });
        result.RenderRejectButton.Should().BeFalse();
    }

    [Test]
    public async Task GetRejectApplicationsViewModel_Returns_A_Valid_ViewModel()
    {
        var request = new RejectApplicationsRequest
        {
            EncodedAccountId = _encodedAccountId,
            EncodedPledgeId = _encodedPledgeId,
            ApplicationsToReject = ["9RMK6Y"]
        };

        var response = _fixture.Create<GetRejectApplicationsResponse>();
        response.Applications = new List<GetRejectApplicationsResponse.Application>
        {
            new()
            {
                Id = 4,
                DasAccountName = "Mega Corp"
            },
            new()
            {
                Id = 5,
                DasAccountName = "Mega Corp"
            }
        };

        _pledgeService.Setup(o => o.GetRejectApplications(request.AccountId, request.PledgeId)).ReturnsAsync(response);
        _encodingService.Setup(x => x.Decode("9RMK6Y", EncodingType.PledgeApplicationId)).Returns(4);

        var result = await _orchestrator.GetRejectApplicationsViewModel(request);

        result.EncodedAccountId.Should().Be(_encodedAccountId);
        result.EncodedPledgeId.Should().Be(_encodedPledgeId);
        result.DasAccountNames.Should().AllBe("Mega Corp");
    }


    [TestCase(true, true)]
    [TestCase(false, false)]
    public async Task GetApplications_Returns_Valid_ViewModel_With_UserCanClosePledge(bool ownerOrTransactorStatus,
        bool expectWhetherUserCanClosePledges)
    {
        var response = new GetApplicationsResponse
        {
            Items = new List<GetApplicationsResponse.Application>
            {
                new()
                {
                    Id = 0,
                    StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                    Status = ApplicationStatus.Pending
                }
            }
        };

        _userService.Setup(x => x.IsOwnerOrTransactor(_encodedAccountId)).Returns(ownerOrTransactorStatus);
        _pledgeService.Setup(x => x.GetApplications(0, 0, 1, null, null, ApplicationsRequest.PageSize)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplications(new ApplicationsRequest
            { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });

        result.UserCanClosePledge.Should().Be(expectWhetherUserCanClosePledges);
    }

    [Test]
    public async Task GetApplication_Returns_ValidViewModel()
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 1000;
        response.Amount = 1;
        response.Status = ApplicationStatus.Pending;

        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0, EncodedAccountId = _encodedAccountId });

        result.JobRole.Should().NotBeNullOrWhiteSpace();
        result.AllowApproval.Should().BeTrue();
        result.AllowRejection.Should().BeTrue();
    }

    [TestCase(100, 0, true)]
    [TestCase(100, 100, true)]
    [TestCase(100, 101, false)]
    public async Task GetApplication_AllowApproval_If_Application_Is_Affordable(int remainingPledgeAmount,
        int applicationAmount, bool expectAllowApproval)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = remainingPledgeAmount;
        response.Amount = applicationAmount;
        response.Status = ApplicationStatus.Pending;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0, EncodedAccountId = _encodedAccountId });

        result.AllowApproval.Should().Be(expectAllowApproval);
    }

    [TestCase(ApplicationStatus.Approved)]
    [TestCase(ApplicationStatus.Accepted)]
    [TestCase(ApplicationStatus.Declined)]
    [TestCase(ApplicationStatus.Withdrawn)]
    public async Task GetApplication_DisallowApproval_If_Application_Is_Not_Pending_Outcome(ApplicationStatus status)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 1000;
        response.Amount = 1;
        response.Status = status;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.AllowApproval.Should().BeFalse();
    }

    [TestCase(ApplicationStatus.Approved)]
    [TestCase(ApplicationStatus.Accepted)]
    [TestCase(ApplicationStatus.Declined)]
    [TestCase(ApplicationStatus.Withdrawn)]
    public async Task GetApplication_DisallowRejection_If_Application_Is_Not_Pending_Outcome(ApplicationStatus status)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 1000;
        response.Amount = 1;
        response.Status = status;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.AllowRejection.Should().BeFalse();
    }

    [Test]
    public async Task GetApplication_DisallowApproval_If_Application_Is_User_Is_Not_Owner_Or_Transactor_Of_Account()
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 1000;
        response.Amount = 1;
        response.Status = ApplicationStatus.Pending;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        _userService.Setup(x => x.IsOwnerOrTransactor(It.IsAny<string>())).Returns(false);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.AllowApproval.Should().BeFalse();
    }

    [Test]
    public async Task GetApplication_DisallowRejection_If_Application_Is_User_Is_Not_Owner_Or_Transactor_Of_Account()
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 1000;
        response.Amount = 1;
        response.Status = ApplicationStatus.Pending;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        _userService.Setup(x => x.IsOwnerOrTransactor(It.IsAny<string>())).Returns(false);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.AllowRejection.Should().BeFalse();
    }

    [TestCase(true, "SelectedAction-2")]
    [TestCase(false, "SelectedAction")]
    public async Task GetApplication_RejectOption_ElementId_Depends_On_Availability_Of_Approval_Option(
        bool allowApproval, string expectedRejectOptionElementId)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.PledgeRemainingAmount = 100;
        response.Amount = allowApproval ? 100 : 200;
        response.Status = ApplicationStatus.Pending;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0, EncodedAccountId = _encodedAccountId });

        result.RejectOptionElementId.Should().Be(expectedRejectOptionElementId);
    }

    [Test]
    public async Task SetApplicationOutcome_Sets_Outcome_Of_Application()
    {
        var request = _fixture.Create<ApplicationPostRequest>();

        await _orchestrator.SetApplicationOutcome(request);

        _pledgeService.Verify(x => x.SetApplicationOutcome(request.AccountId,
            request.ApplicationId,
            request.PledgeId,
            It.Is<SetApplicationOutcomeRequest>(r =>
                r.UserId == _userId &&
                r.UserDisplayName == _userDisplayName &&
                r.Outcome == (request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve
                    ? SetApplicationOutcomeRequest.ApplicationOutcome.Approve
                    : SetApplicationOutcomeRequest.ApplicationOutcome.Reject)
            )));
    }

    [Test]
    public async Task Close_Pledge_Request()
    {
        var request = _fixture.Create<ClosePostRequest>();

        await _orchestrator.ClosePledge(request);

        _pledgeService.Verify(x => x.ClosePledge(request.AccountId,
            request.PledgeId,
            It.Is<ClosePledgeRequest>(r =>
                r.UserId == _userId &&
                r.UserDisplayName == _userDisplayName)));
    }

    [TestCase("www.contoso.com", "http://www.contoso.com")]
    [TestCase("http://www.contoso.com", "http://www.contoso.com")]
    [TestCase("https://www.contoso.com", "https://www.contoso.com")]
    [TestCase("", "")]
    [TestCase(null, null)]
    public async Task GetApplication_Adds_Url_Prefix_If_Missing(string url, string expectedUrl)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.BusinessWebsite = url;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.BusinessWebsite.Should().Be(expectedUrl);
    }

    [Test]
    public void GetAffordabilityViewModel_Returns_Correct_Values()
    {
        var remainingAmount = _fixture.Create<int>();
        var numberOfApprentices = _fixture.Create<int>();
        var maxFunding = _fixture.Create<int>();
        var estimatedDurationMonths = _fixture.Create<int>();
        var startDate = _fixture.Create<DateTime>();

        var firstYearCost = estimatedDurationMonths <= 12
            ? maxFunding * numberOfApprentices
            : (((decimal)(maxFunding * 0.8) * numberOfApprentices) / estimatedDurationMonths) * 12;

        var expectedRemainingFundsIfApproved = Math.Round(remainingAmount - firstYearCost);
        var expectedEstimatedCostOverDuration = maxFunding * numberOfApprentices;

        var viewModel = _orchestrator.GetAffordabilityViewModel(remainingAmount, numberOfApprentices, maxFunding,
            estimatedDurationMonths, startDate);

        viewModel.RemainingFundsIfApproved.Should().Be((int)expectedRemainingFundsIfApproved);
        viewModel.EstimatedCostOverDuration.Should().Be(expectedEstimatedCostOverDuration);
        viewModel.YearDescription.Should().Be(_dateTimeService.Object.UtcNow.ToTaxYearDescription());
        viewModel.YearlyPayments.Should().NotBeEmpty();
    }

    [Test]
    public void GetAffordabilityViewModel_Returns_Correct_YearlyPayments_Values()
    {
        const int remainingAmount = 172;
        const int numberOfApprentices = 2;
        const int maxFunding = 28000;
        const int estimatedDurationMonths = 138;
        var startDate = new DateTime(2023, 9, 1);
        const double completionPayment = maxFunding * numberOfApprentices * 0.2;

        var expectedPayments = new List<YearlyPayments>
        {
            new("first year", 3896),
            new("second year", 3896),
            new("third year", 3896),
            new("fourth year", 3896),
            new("fifth year", 3896),
            new("sixth year", 3896),
            new("seventh year", 3896),
            new("eighth year", 3896),
            new("ninth year", 3896),
            new("tenth year", 3896),
            new("eleventh year", 3896),
            new("final year", (3896 / 2) + (int)completionPayment)
        };

        var viewModel = _orchestrator.GetAffordabilityViewModel(remainingAmount, numberOfApprentices, maxFunding,
            estimatedDurationMonths, startDate);

        viewModel.YearlyPayments.Should().HaveCount(12);

        for (var index = 0; index < viewModel.YearlyPayments.Count; index++)
        {
            viewModel.YearlyPayments[index].Year.Should().Be(expectedPayments[index].Year);
            viewModel.YearlyPayments[index].Amount.Should().Be(expectedPayments[index].Amount);
        }
    }

    [Test]
    public async Task GetApplicationApprovedViewModel_EncodedAccountId_Is_Correct()
    {
        var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest
        {
            EncodedAccountId = _encodedAccountId,
            EncodedPledgeId = _encodedPledgeId,
            EncodedApplicationId = _encodedApplicationId,
            AccountId = _accountId,
            PledgeId = _pledgeId,
            ApplicationId = _applicationId
        });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetApplicationApprovedViewModel_EncodedPledgeId_Has_Value()
    {
        var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest
        {
            EncodedAccountId = _encodedAccountId,
            EncodedPledgeId = _encodedPledgeId,
            EncodedApplicationId = _encodedApplicationId,
            AccountId = _accountId,
            PledgeId = _pledgeId,
            ApplicationId = _applicationId
        });
        result.EncodedPledgeId.Should().Be(_encodedPledgeId);
    }

    [Test]
    public async Task GetApplicationApprovedViewModel_DasAccountName_Has_Value()
    {
        var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest
        {
            EncodedAccountId = _encodedAccountId,
            EncodedPledgeId = _encodedAccountId,
            EncodedApplicationId = _encodedApplicationId,
            AccountId = _accountId,
            PledgeId = _pledgeId,
            ApplicationId = _applicationId
        });
        result.DasAccountName.Should().Be(_applicationApprovedResponse.EmployerAccountName);
    }

    [Test]
    public async Task GetPledgeApplicationsDownloadModel_Retrieves_ApplicationsFromService()
    {
        var accountId = _fixture.Create<int>();
        var getPledgeApplicationsResponse = _fixture.Create<GetApplicationsResponse>();
        _pledgeService.Setup(o =>
                o.GetApplications(It.Is<long>(l => l == accountId),
                    It.Is<int>(p => p == _pledgeId), It.IsAny<int>(), null, null, It.IsAny<int?>()))
            .ReturnsAsync(getPledgeApplicationsResponse);

        await _orchestrator.GetPledgeApplicationsDownloadModel(new ApplicationsRequest
        {
            AccountId = accountId,
            PledgeId = _pledgeId
        });

        _pledgeService.Verify(
            o => o.GetApplications(It.Is<long>(l => l == accountId), It.Is<int>(p => p == _pledgeId), It.IsAny<int>(), null, null, It.IsAny<int?>()),
            Times.Once);
    }

    [TestCase(0, "pink")]
    [TestCase(25, "pink")]
    [TestCase(50, "yellow")]
    [TestCase(75, "yellow")]
    [TestCase(100, "turquoise")]
    [Test]
    public async Task GetApplication_PercentageMatchCssClass_Is_Correct(int matchPercentage, string expectedResult)
    {
        var response = _fixture.Create<GetApplicationResponse>();
        response.MatchPercentage = matchPercentage;
        _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest
            { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

        result.PercentageMatchCssClass.Should().Be(expectedResult);
    }
}