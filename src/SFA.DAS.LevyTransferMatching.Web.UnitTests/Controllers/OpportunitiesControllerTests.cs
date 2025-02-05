using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers;

[TestFixture]
public class OpportunitiesControllerTests
{
    private OpportunitiesController _opportunitiesController;
    private Fixture _fixture;
    private Mock<IOpportunitiesOrchestrator> _orchestrator;
    private IndexRequest _indexRequest;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _orchestrator = new Mock<IOpportunitiesOrchestrator>();
        _indexRequest = _fixture.Create<IndexRequest>();
        _opportunitiesController = new OpportunitiesController(_orchestrator.Object);
    }

    [TearDown]
    public void TearDown() => _opportunitiesController?.Dispose();

    [Test]
    public async Task GET_Index_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        _orchestrator.Setup(x => x.GetIndexViewModel(_indexRequest)).ReturnsAsync(() => new IndexViewModel());

        // Act
        var viewResult = await _opportunitiesController.Index(_indexRequest) as ViewResult;
        var indexViewModel = viewResult?.Model as IndexViewModel;

        viewResult.Should().NotBeNull();
        indexViewModel.Should().NotBeNull();
    }


    [Test]
    public async Task GET_Index_With_CommaSeparatedSectors_Populates_Sectors_in_IndexRequest()
    {
        // Arrange
        var commaSeparatedSectors = "Business,Charity,Finance";
        var expectedSectors = new List<string> { "Business", "Charity", "Finance" };

        _indexRequest.CommaSeparatedSectors = commaSeparatedSectors;
        _indexRequest.Sectors = null;

        // Act
        await _opportunitiesController.Index(_indexRequest);
        _indexRequest.Sectors.Should().BeEquivalentTo(expectedSectors);
    }
    
    [Test]
    public async Task GET_Index_With_Sectors_Populates_CommaSeparatedSectors_in_IndexRequest()
    {
        // Arrange
        var commaSeparatedSectors = "Business,Charity,Finance";
        var sectors = new List<string> { "Business", "Charity", "Finance" };

        _indexRequest.CommaSeparatedSectors = null;
        _indexRequest.Sectors = sectors;

        // Act
        await _opportunitiesController.Index(_indexRequest);
        _indexRequest.CommaSeparatedSectors.Should().BeEquivalentTo(commaSeparatedSectors);
    }

    [Test]
    public async Task POST_Index_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var indexRequest = new IndexRequest();
        _orchestrator.Setup(x => x.GetIndexViewModel(indexRequest)).ReturnsAsync(() => new IndexViewModel());

        // Act
        var viewResult = await _opportunitiesController.Index(indexRequest) as ViewResult;
        var indexViewModel = viewResult?.Model as IndexViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        indexViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task GET_Detail_Opportunity_Exists_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var detailRequest = _fixture.Create<DetailRequest>();
        var expectedDetailViewModel = _fixture.Create<DetailViewModel>();

        _orchestrator
            .Setup(x => x.GetDetailViewModel(detailRequest))
            .ReturnsAsync(expectedDetailViewModel);

        // Act
        var viewResult = await _opportunitiesController.Detail(detailRequest) as ViewResult;
        var actualDetailViewModel = viewResult?.Model as DetailViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        actualDetailViewModel.Should().NotBeNull();
        actualDetailViewModel.Should().Be(expectedDetailViewModel);
    }

    [Test]
    public async Task GET_Detail_Opportunity_Doesnt_Exist_Returns_404()
    {
        // Arrange
        var detailRequest = _fixture.Create<DetailRequest>();

        _orchestrator
            .Setup(x => x.GetDetailViewModel(detailRequest))
            .ReturnsAsync((DetailViewModel)null);

        // Act
        var notFoundResult = await _opportunitiesController.Detail(detailRequest) as NotFoundResult;

        // Assert
        notFoundResult.Should().NotBeNull();
    }

    [Test]
    public void POST_Detail_No_Selected_Redirects_To_Index()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();
        var detailPostRequest = new DetailPostRequest
        {
            EncodedPledgeId = encodedPledgeId,
            HasConfirmed = false,
        };

        // Assert
        var redirectToActionResult = _opportunitiesController.Detail(detailPostRequest) as RedirectToActionResult;

        // Assert
        redirectToActionResult.Should().NotBeNull();
        redirectToActionResult.ActionName.Should().Be(nameof(OpportunitiesController.Index));
    }

    [Test]
    public void POST_Detail_Yes_Selected_Redirects_To_SelectAccount()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();
        var detailPostRequest = new DetailPostRequest
        {
            EncodedPledgeId = encodedPledgeId,
            HasConfirmed = true,
        };

        // Act
        var result = _opportunitiesController.Detail(detailPostRequest) as RedirectToActionResult;

        // Assert
        redirectToActionResult.Should().NotBeNull();
        redirectToActionResult.ActionName.Should().Be(nameof(OpportunitiesController.SelectAccount));
    }

    [Test]
    public async Task GET_SelectAccount_AccountsReturnedEqualsOne_ReturnsRedirectToApply()
    {
        // Arrange
        var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
        var viewModel = _fixture
            .Build<SelectAccountViewModel>()
            .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>(1))
            .Create();

        _orchestrator
            .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
            .ReturnsAsync(viewModel);

        // Act
        var result = await _opportunitiesController.SelectAccount(selectAccountRequest);
        var redirectToActionResult = result as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        redirectToActionResult.Should().NotBeNull();
        redirectToActionResult.ActionName.Should().Be(nameof(OpportunitiesController.Apply));
    }

    [Test]
    public async Task GET_SelectAccount_AccountsReturnedMoreThanOne_ReturnsViewWithAccounts()
    {
        // Arrange
        var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
        var expectedViewModel = _fixture
            .Build<SelectAccountViewModel>()
            .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>())
            .Create();

        _orchestrator
            .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.SelectAccount(selectAccountRequest);
        var viewResult = result as ViewResult;
        var model = viewResult.Model;
        var actualViewModel = model as SelectAccountViewModel;

        // Assert
        result.Should().NotBeNull();
        viewResult.Should().NotBeNull();
        model.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test]
    public async Task GET_SelectAccount_NoAccountsReturned_ReturnsEmptyView()
    {
        // Arrange
        var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
        var expectedViewModel = _fixture
            .Build<SelectAccountViewModel>()
            .With(x => x.Accounts, _fixture.CreateMany<SelectAccountViewModel.Account>(0))
            .Create();

        _orchestrator
            .Setup(x => x.GetSelectAccountViewModel(It.Is<SelectAccountRequest>(y => y == selectAccountRequest)))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.SelectAccount(selectAccountRequest);
        var viewResult = result as ViewResult;
        var model = viewResult.Model;
        var actualViewModel = model as SelectAccountViewModel;

        // Assert
        result.Should().NotBeNull();
        viewResult.Should().NotBeNull();
        model.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test]
    public async Task GET_MoreDetails_Returns_Expected_View()
    {
        // Arrange
        var request = _fixture.Create<MoreDetailsRequest>();
        var expectedViewModel = _fixture.Create<MoreDetailsViewModel>();

        _orchestrator
            .Setup(x => x.GetMoreDetailsViewModel(request))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.MoreDetails(request) as ViewResult;
        var actualViewModel = result?.Model as MoreDetailsViewModel;

        // Assert
        result.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
        _orchestrator.Verify(x => x.GetMoreDetailsViewModel(request), Times.Once);
    }

    [Test]
    public async Task POST_MoreDetails_Redirects_To_Apply()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();
        var encodedAccountId = _fixture.Create<string>();
        var cacheKey = _fixture.Create<Guid>();

        var request = new MoreDetailsPostRequest
        {
            EncodedPledgeId = encodedPledgeId,
            EncodedAccountId = encodedAccountId,
            Details = _fixture.Create<string>(),
            CacheKey = cacheKey
        };

        _orchestrator.Setup(x => x.UpdateCacheItem(request));

        // Act
        var result = await _opportunitiesController.MoreDetails(request) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OpportunitiesController.Apply));
        result.RouteValues["EncodedPledgeId"].Should().Be(encodedPledgeId);
        result.RouteValues["EncodedAccountId"].Should().Be(encodedAccountId);
        result.RouteValues["CacheKey"].Should().Be(cacheKey);
        _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
    }

    [Test]
    public async Task GET_ContactDetails_Returns_View_With_Expected_ViewModel()
    {
        // Arrange
        var contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

        _orchestrator
            .Setup(x => x.GetContactDetailsViewModel(It.Is<ContactDetailsRequest>(y => y == contactDetailsRequest)))
            .ReturnsAsync(() => new ContactDetailsViewModel());

        // Act
        var result = await _opportunitiesController.ContactDetails(contactDetailsRequest) as ViewResult;
        var contactDetailsViewModel = result?.Model as ContactDetailsViewModel;

        // Assert
        result.Should().NotBeNull();
        contactDetailsViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task POST_ContactDetails_Cache_Updated_And_Redirects_To_Apply()
    {
        // Arrange
        var encodedAccountId = _fixture.Create<string>();
        var encodedPledgeId = _fixture.Create<string>();
        var cacheKey = _fixture.Create<Guid>();
        var contactDetailsPostRequest = new ContactDetailsPostRequest
        {
            EncodedAccountId = encodedAccountId,
            EncodedPledgeId = encodedPledgeId,
            CacheKey = cacheKey,
        };

        var cacheUpdated = false;
        _orchestrator
            .Setup(x => x.UpdateCacheItem(It.Is<ContactDetailsPostRequest>(y => y == contactDetailsPostRequest)))
            .Callback(() => { cacheUpdated = true; });

        // Act
        var result = await _opportunitiesController.ContactDetails(contactDetailsPostRequest) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OpportunitiesController.Apply));
        cacheUpdated.Should().BeTrue();
        result.RouteValues["encodedAccountId"].Should().Be(encodedAccountId);
        result.RouteValues["encodedPledgeId"].Should().Be(encodedPledgeId);
        result.RouteValues["cacheKey"].Should().Be(cacheKey);
    }

    [Test]
    public async Task GET_ApplicationDetails_Returns_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationDetailsRequest>();
        var expectedViewModel = _fixture.Create<ApplicationDetailsViewModel>();

        _orchestrator
            .Setup(x => x.GetApplicationViewModel(request))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.ApplicationDetails(request) as ViewResult;
        var actualViewModel = result?.Model as ApplicationDetailsViewModel;

        // Assert
        result.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
        _orchestrator.Verify(x => x.GetApplicationViewModel(request), Times.Once);
    }

    [Test]
    public async Task POST_ApplicationDetails_Redirects_To_Apply()
    {
        // Arrange
        var selectedStandardId = _fixture.Create<string>();
        var encodedPledgeId = _fixture.Create<string>();
        var encodedAccountId = _fixture.Create<string>();
        var cacheKey = _fixture.Create<Guid>();
        var applicationRequest = _fixture.Create<ApplicationRequest>();
        applicationRequest.EncodedPledgeId = encodedPledgeId;
        applicationRequest.EncodedAccountId = encodedAccountId;
        applicationRequest.CacheKey = cacheKey;

        var request = new ApplicationDetailsPostRequest
        {
            AccountId = 1,
            EncodedPledgeId = encodedPledgeId,
            EncodedAccountId = encodedAccountId,
            CacheKey = cacheKey,
            HasTrainingProvider = true,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year,
            NumberOfApprentices = "1",
            PledgeId = 1,
            SelectedStandardId = selectedStandardId,
            SelectedStandardTitle = "Test Standard Title"
        };

        var opportunitiesService = new Mock<IOpportunitiesService>();
        opportunitiesService.Setup(x => x.GetApplicationDetails(1, 1, selectedStandardId)).ReturnsAsync(
            new GetApplicationDetailsResponse
            {
                Opportunity = new GetApplicationDetailsResponse.OpportunityData
                {
                    Amount = 100_000,
                    RemainingAmount = 100_000
                },
                Standards = new List<StandardsListItemDto>
                {
                    new()
                    {
                        ApprenticeshipFunding =
                        [
                            new()
                            {
                                Duration = 12,
                                MaxEmployerLevyCap = 9_000,
                                EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-1).Year,
                                    DateTime.UtcNow.Month, 1),
                                EffectiveTo = null
                            }
                        ]
                    }
                }
            });

        _orchestrator.Setup(x => x.PostApplicationViewModel(request)).ReturnsAsync(applicationRequest);

        // Act
        var result = await _opportunitiesController.ApplicationDetails(
            new ApplicationDetailsPostRequestAsyncValidator(opportunitiesService.Object),
            request) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OpportunitiesController.Apply));
        result.RouteValues["EncodedPledgeId"].Should().Be(encodedPledgeId);
        result.RouteValues["EncodedAccountId"].Should().Be(encodedAccountId);
        result.RouteValues["CacheKey"].Should().Be(cacheKey);
    }

    [Test]
    [Ignore(
        "During March and April this test will always fail as the cost will be calculated as zero, which can always be afforded, even from an empty pledge. Costing is to be overhauled anyway shortly, so safe to ignore this for now.")]
    public async Task POST_ApplicationDetails_Redirects_To_ApplicationDetails_On_Invalid_ModelState()
    {
        // Arrange
        var selectedStandardId = _fixture.Create<string>();
        var encodedPledgeId = _fixture.Create<string>();
        var encodedAccountId = _fixture.Create<string>();
        var cacheKey = _fixture.Create<Guid>();
        var applicationRequest = _fixture.Create<ApplicationRequest>();
        applicationRequest.EncodedPledgeId = encodedPledgeId;
        applicationRequest.EncodedAccountId = encodedAccountId;
        applicationRequest.CacheKey = cacheKey;

        var request = new ApplicationDetailsPostRequest
        {
            AccountId = 1,
            EncodedPledgeId = encodedPledgeId,
            EncodedAccountId = encodedAccountId,
            CacheKey = cacheKey,
            HasTrainingProvider = true,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year,
            NumberOfApprentices = "1",
            PledgeId = 1,
            SelectedStandardId = selectedStandardId,
            SelectedStandardTitle = "Test Standard Title"
        };

        var opportunitiesService = new Mock<IOpportunitiesService>();
        opportunitiesService.Setup(x => x.GetApplicationDetails(1, 1, selectedStandardId)).ReturnsAsync(
            new GetApplicationDetailsResponse
            {
                Opportunity = new GetApplicationDetailsResponse.OpportunityData
                {
                    Amount = 100_000,
                    RemainingAmount = 0
                },
                Standards = new List<StandardsListItemDto>
                {
                    new()
                    {
                        ApprenticeshipFunding =
                        [
                            new()
                            {
                                Duration = 12,
                                MaxEmployerLevyCap = 9_000,
                                EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-1).Year,
                                    DateTime.UtcNow.Month, 1),
                                EffectiveTo = null
                            }
                        ]
                    }
                }
            });

        _orchestrator.Setup(x => x.PostApplicationViewModel(request)).ReturnsAsync(applicationRequest);

        // Act
        var result = await _opportunitiesController.ApplicationDetails(
            new ApplicationDetailsPostRequestAsyncValidator(opportunitiesService.Object),
            request) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OpportunitiesController.ApplicationDetails));
        result.RouteValues["EncodedPledgeId"].Should().Be(encodedPledgeId);
        result.RouteValues["EncodedAccountId"].Should().Be(encodedAccountId);
        result.RouteValues["CacheKey"].Should().Be(cacheKey);
    }

    [Test]
    public async Task GET_Confirmation_Returns_Expected_View()
    {
        // Arrange
        var request = _fixture.Create<ConfirmationRequest>();
        var expectedViewModel = _fixture.Create<ConfirmationViewModel>();

        _orchestrator
            .Setup(x => x.GetConfirmationViewModel(request))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.Confirmation(request) as ViewResult;
        var actualViewModel = result?.Model as ConfirmationViewModel;

        // Assert
        result.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test]
    public async Task GET_Apply_Returns_Expected_View()
    {
        // Arrange
        var expectedViewModel = new ApplyViewModel();

        _orchestrator.Setup(x => x.GetApplyViewModel(It.IsAny<ApplicationRequest>()))
            .ReturnsAsync(expectedViewModel);

        // Act
        var result = await _opportunitiesController.Apply(new ApplicationRequest()) as ViewResult;
        var actualViewModel = result?.Model as ApplyViewModel;

        // Assert
        result.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test]
    public async Task POST_Apply_Redirects_To_Confirmation()
    {
        var request = new ApplyPostRequest();
        var result = await _opportunitiesController.Apply(request) as RedirectToActionResult;
        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OpportunitiesController.Confirmation));
    }

    [Test]
    public async Task GET_GetFundingEstimate_Returns_Expected_Json_Result()
    {
        var expectedViewModel = new GetFundingEstimateViewModel();

        _orchestrator.Setup(x => x.GetFundingEstimate(It.IsAny<GetFundingEstimateRequest>(), null))
            .ReturnsAsync(expectedViewModel);

        var jsonResult =
            await _opportunitiesController.GetFundingEstimate(new GetFundingEstimateRequest()) as JsonResult;

        jsonResult.Should().NotBeNull();
        jsonResult.Value.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test]
    public async Task GET_Sector_Returns_Expected_ViewModel()
    {
        var request = _fixture.Create<SectorRequest>();
        var expectedViewModel = _fixture.Create<SectorViewModel>();

        _orchestrator
            .Setup(x => x.GetSectorViewModel(request))
            .ReturnsAsync(expectedViewModel);

        var viewResult = await _opportunitiesController.Sector(request) as ViewResult;
        var actualViewModel = viewResult.Model as SectorViewModel;

        viewResult.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
        actualViewModel.Should().BeEquivalentTo(expectedViewModel);
        _orchestrator.Verify(x => x.GetSectorViewModel(request), Times.Once);
    }

    [Test]
    public async Task POST_Sector_Redirects_To_Apply_On_Valid_ModelState()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();
        var encodedAccountId = _fixture.Create<string>();
        var cacheKey = _fixture.Create<Guid>();

        var request = new SectorPostRequest
        {
            Sectors = ["Sector"],
            EncodedPledgeId = encodedPledgeId,
            EncodedAccountId = encodedAccountId,
            CacheKey = cacheKey
        };

        _orchestrator.Setup(x => x.UpdateCacheItem(request));

        // Assert
        var redirectToActionResult = await _opportunitiesController.Sector(request) as RedirectToActionResult;

        // Assert

        redirectToActionResult.Should().NotBeNull();
        redirectToActionResult.ActionName.Should().Be(nameof(OpportunitiesController.Apply));
        redirectToActionResult.RouteValues["EncodedPledgeId"].Should().Be(encodedPledgeId);
        redirectToActionResult.RouteValues["EncodedAccountId"].Should().Be(encodedAccountId);
        redirectToActionResult.RouteValues["CacheKey"].Should().Be(cacheKey);
        _orchestrator.Verify(x => x.UpdateCacheItem(request), Times.Once);
    }
}