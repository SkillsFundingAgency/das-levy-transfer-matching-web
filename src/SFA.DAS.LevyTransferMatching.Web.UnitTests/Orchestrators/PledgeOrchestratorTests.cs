using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Services;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;
using ApplicationRequest = SFA.DAS.LevyTransferMatching.Web.Models.Pledges.ApplicationRequest;
using GetApplicationsResponse = SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types.GetApplicationsResponse;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
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
        private Guid _cacheKey;
        private readonly long _accountId = 1;
        private readonly int _pledgeId = 1;
        private string _encodedPledgeId;
        private string _encodedApplicationId;
        private readonly int _applicationId = 1;
        private string _userId;
        private string _userDisplayName;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _encodedAccountId = _fixture.Create<string>();
            _cacheKey = Guid.NewGuid();
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
            _applicationApprovedResponse = _fixture.Create<GetApplicationApprovedResponse>();

            _encodedPledgeId = _fixture.Create<string>();
            _encodedApplicationId = _fixture.Create<string>();

            _pledgeService.Setup(x => x.GetPledges(_accountId)).ReturnsAsync(_pledgesResponse);
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
            _pledgeService.Setup(x => x.GetApplicationApproved(_accountId, _pledgeId, _applicationId)).ReturnsAsync(_applicationApprovedResponse);

            _userId = _fixture.Create<string>();
            _userDisplayName = _fixture.Create<string>();
            _userService.Setup(x => x.IsUserChangeAuthorized()).Returns(true);
            _userService.Setup(x => x.GetUserId()).Returns(_userId);
            _userService.Setup(x => x.GetUserDisplayName()).Returns(_userDisplayName);
            _userService.Setup(x => x.IsOwnerOrTransactor(0)).Returns(true);

            _orchestrator = new PledgeOrchestrator(_pledgeService.Object, _encodingService.Object, _userService.Object, _featureToggles,
                _dateTimeService.Object, _csvService.Object);
        }



        [Test]
        public async Task GetPledgesViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetPledgesViewModel_RenderCreatePledgeButton_Is_True_When_Authorized()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.IsTrue(result.RenderCreatePledgeButton);
        }

        [Test]
        public async Task GetPledgesViewModel_Pledges_Is_Populated()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.NotNull(result.Pledges);
        }


        [Test]
        public async Task GetApplications_Returns_Valid_ViewModel()
        {
            var response = new GetApplicationsResponse()
            {
                Applications = new List<GetApplicationsResponse.Application>()
                {
                    new GetApplicationsResponse.Application()
                    {
                        Id = 0,
                        StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                        Status = ApplicationStatus.Pending
                    }
                }
            };

            _pledgeService.Setup(x => x.GetApplications(0, 0, null, null)).ReturnsAsync(response);
            _encodingService.Setup(x => x.Encode(0, EncodingType.PledgeApplicationId)).Returns("123");

            var result = await _orchestrator.GetApplications(new ApplicationsRequest() { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });

            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(_encodedPledgeId, result.EncodedPledgeId);
            result.Applications.ToList().ForEach(application =>
            {
                Assert.AreEqual("123", application.EncodedApplicationId);
                Assert.AreEqual(ApplicationStatus.Pending, application.Status);
            });
        }

        [Test]
        public async Task GetRejectApplicationsViewModel_Returns_A_Valid_ViewModel()
        {
            var request = new RejectApplicationsRequest {
                EncodedAccountId = _encodedAccountId,
                EncodedPledgeId = _encodedPledgeId
            };
            request.ApplicationsToReject = new List<string> { "9RMK6Y"};
            
            var response = _fixture.Create<GetRejectApplicationsResponse>();
            response.Applications = new List<GetRejectApplicationsResponse.Application>()
            {
                new GetRejectApplicationsResponse.Application()
                {
                     Id = 4,
                     DasAccountName = "Mega Corp"
                },
                new GetRejectApplicationsResponse.Application()
                {
                     Id = 5,
                     DasAccountName = "Mega Corp"
                }
            };

            _pledgeService.Setup(o => o.GetRejectApplications(request.AccountId, request.PledgeId)).ReturnsAsync(response);
            
            _encodingService.Setup(x => x.Decode("9RMK6Y", EncodingType.PledgeApplicationId)).Returns(4);

            var result = await _orchestrator.GetRejectApplicationsViewModel(request);

            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(_encodedPledgeId, result.EncodedPledgeId);

            result.DasAccountNames.ToList().ForEach(application =>
            {
                Assert.AreEqual("Mega Corp", application);
            });
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task GetApplications_Returns_Valid_ViewModel_With_UserCanClosePledge(bool ownerOrTransactorStatus, bool expectWhetherUserCanClosePledges)
        {
            var response = new GetApplicationsResponse()
            {
                Applications = new List<GetApplicationsResponse.Application>()
                {
                    new GetApplicationsResponse.Application()
                    {
                        Id = 0,
                        StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                        Status = ApplicationStatus.Pending
                    }
                }
            };

            _userService.Setup(x => x.IsOwnerOrTransactor(0)).Returns(ownerOrTransactorStatus);
            _pledgeService.Setup(x => x.GetApplications(0, 0, null, null)).ReturnsAsync(response);
           
            var result = await _orchestrator.GetApplications(new ApplicationsRequest() { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId });

            Assert.AreEqual(expectWhetherUserCanClosePledges, result.UserCanClosePledge);
        }

        [Test]
        public async Task GetApplication_Returns_ValidViewModel()
        {
            var response = _fixture.Create<GetApplicationResponse>();
            response.PledgeRemainingAmount = 1000;
            response.Amount = 1;
            response.Status = ApplicationStatus.Pending;

            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.JobRole));
            Assert.IsTrue(result.AllowApproval);
            Assert.IsTrue(result.AllowRejection);
        }

        [TestCase(100, 0, true)]
        [TestCase(100, 100, true)]
        [TestCase(100, 101, false)]
        public async Task GetApplication_AllowApproval_If_Application_Is_Affordable(int remainingPledgeAmount, int applicationAmount, bool expectAllowApproval)
        {
            var response = _fixture.Create<GetApplicationResponse>();
            response.PledgeRemainingAmount = remainingPledgeAmount;
            response.Amount = applicationAmount;
            response.Status = ApplicationStatus.Pending;
            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.AreEqual(expectAllowApproval, result.AllowApproval);
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

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.IsFalse(result.AllowApproval);
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

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.IsFalse(result.AllowRejection);
        }

        [Test]
        public async Task GetApplication_DisallowApproval_If_Application_Is_User_Is_Not_Owner_Or_Transactor_Of_Account()
        {
            var response = _fixture.Create<GetApplicationResponse>();
            response.PledgeRemainingAmount = 1000;
            response.Amount = 1;
            response.Status = ApplicationStatus.Pending;
            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

            _userService.Setup(x => x.IsOwnerOrTransactor(0)).Returns(false);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.IsFalse(result.AllowApproval);
        }

        [Test]
        public async Task GetApplication_DisallowRejection_If_Application_Is_User_Is_Not_Owner_Or_Transactor_Of_Account()
        {
            var response = _fixture.Create<GetApplicationResponse>();
            response.PledgeRemainingAmount = 1000;
            response.Amount = 1;
            response.Status = ApplicationStatus.Pending;
            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

            _userService.Setup(x => x.IsOwnerOrTransactor(0)).Returns(false);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.IsFalse(result.AllowRejection);
        }

        [TestCase(true, "SelectedAction-2")]
        [TestCase(false, "SelectedAction")]
        public async Task GetApplication_RejectOption_ElementId_Depends_On_Availability_Of_Approval_Option(bool allowApproval, string expectedRejectOptionElementId)
        {
            var response = _fixture.Create<GetApplicationResponse>();
            response.PledgeRemainingAmount = 100;
            response.Amount = allowApproval ? 100 : 200;
            response.Status = ApplicationStatus.Pending;
            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.AreEqual(expectedRejectOptionElementId, result.RejectOptionElementId);
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

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.AreEqual(expectedUrl, result.BusinessWebsite);
        }

        [Test]
        public void GetAffordabilityViewModel_Returns_Correct_Values()
        {
            var amount = _fixture.Create<int>();
            var remainingAmount = _fixture.Create<int>();
            var numberOfApprentices = _fixture.Create<int>();
            var maxFunding = _fixture.Create<int>();
            var estimatedDurationMonths = _fixture.Create<int>();
            var startDate = _fixture.Create<DateTime>();

            var expectedRemainingFundsIfApproved = remainingAmount - amount;
            var expectedEstimatedCostOverDuration = maxFunding * numberOfApprentices;

            var viewModel = _orchestrator.GetAffordabilityViewModel(amount, remainingAmount, numberOfApprentices, maxFunding, estimatedDurationMonths, startDate);

            Assert.AreEqual(remainingAmount.ToCurrencyString(), viewModel.RemainingFunds);
            Assert.AreEqual(amount.ToCurrencyString(), viewModel.EstimatedCostThisYear);
            Assert.AreEqual(expectedRemainingFundsIfApproved.ToCurrencyString(), viewModel.RemainingFundsIfApproved);
            Assert.AreEqual(expectedEstimatedCostOverDuration.ToCurrencyString(), viewModel.EstimatedCostOverDuration);
            Assert.AreEqual(_dateTimeService.Object.UtcNow.ToTaxYearDescription(), viewModel.YearDescription);
        }


        [Test]
        public async Task GetApplicationApprovedViewModel_EncodedAccountId_Is_Correct()
        {
            var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId, EncodedApplicationId = _encodedApplicationId, AccountId = _accountId, PledgeId = _pledgeId, ApplicationId = _applicationId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetApplicationApprovedViewModel_EncodedPledgeId_Has_Value()
        {
            var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId, EncodedApplicationId = _encodedApplicationId, AccountId = _accountId, PledgeId = _pledgeId, ApplicationId = _applicationId });
            Assert.AreEqual(_encodedPledgeId, result.EncodedPledgeId);
        }

        [Test]
        public async Task GetApplicationApprovedViewModel_DasAccountName_Has_Value()
        {
            var result = await _orchestrator.GetApplicationApprovedViewModel(new ApplicationApprovedRequest { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedAccountId, EncodedApplicationId = _encodedApplicationId, AccountId = _accountId, PledgeId = _pledgeId, ApplicationId = _applicationId });
            Assert.AreEqual(_applicationApprovedResponse.EmployerAccountName, result.DasAccountName);
        }

        [Test]
        public async Task GetPledgeApplicationsDownloadModel_Retrieves_ApplicationsFromService()
        {
            var accountId = _fixture.Create<int>();
            var getPledgeApplicationsResponse = _fixture.Create<GetApplicationsResponse>();
            _pledgeService.Setup(o =>
                o.GetApplications(It.Is<long>(l => l == accountId),
                    It.Is<int>(p => p == _pledgeId), null, null))
                .ReturnsAsync(getPledgeApplicationsResponse);

            await _orchestrator.GetPledgeApplicationsDownloadModel(new ApplicationsRequest
            {
                AccountId = accountId, PledgeId = _pledgeId
            });

            _pledgeService.Verify(o => o.GetApplications(It.Is<long>(l => l == accountId), It.Is<int>(p => p == _pledgeId), null, null), Times.Once);
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

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0 });

            Assert.AreEqual(result.PercentageMatchCssClass, expectedResult);
        }
    }
}
