using FluentAssertions.Execution;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers;

[TestFixture]
public class CreatePledgeControllerTests
{
    private CreatePledgeController _pledgesController;
    private Mock<ICreatePledgeOrchestrator> _orchestrator;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _orchestrator = new Mock<ICreatePledgeOrchestrator>();
        _pledgesController = new CreatePledgeController(_orchestrator.Object);
    }

    [TearDown]
    public void TearDown() => _pledgesController?.Dispose();

    [Test]
    public void GET_Inform_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var encodedAccountId = _fixture.Create<string>();
        _orchestrator.Setup(x => x.GetInformViewModel(encodedAccountId)).Returns(() => new InformViewModel());

        // Act
        var viewResult = _pledgesController.Inform(encodedAccountId) as ViewResult;
        var indexViewModel = viewResult?.Model as InformViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            indexViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task GET_Create_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<CreateRequest>();
        _orchestrator.Setup(x => x.GetCreateViewModel(request)).ReturnsAsync(() => new CreateViewModel());

        // Act
        var viewResult = await _pledgesController.Create(request) as ViewResult;
        var createViewModel = viewResult?.Model as CreateViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            createViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task GET_Amount_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<AmountRequest>();
        _orchestrator.Setup(x => x.GetAmountViewModel(request)).ReturnsAsync(() => new AmountViewModel());

        // Act
        var viewResult = await _pledgesController.Amount(request) as ViewResult;
        var amountViewModel = viewResult?.Model as AmountViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            amountViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_Amount_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<AmountPostRequest>();

        // Act
        var actionResult = await _pledgesController.Amount(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_OrganisationName_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<OrganisationNameRequest>();
        _orchestrator.Setup(x => x.GetOrganisationNameViewModel(request))
            .ReturnsAsync(() => new OrganisationNameViewModel());

        // Act
        var viewResult = await _pledgesController.Organisation(request) as ViewResult;
        var organisationNameViewModel = viewResult?.Model as OrganisationNameViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            organisationNameViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_OrganisationName_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<OrganisationNamePostRequest>();

        // Act
        var actionResult = await _pledgesController.Organisation(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_AutoApproval_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<AutoApproveRequest>();
        _orchestrator.Setup(x => x.GetAutoApproveViewModel(request)).ReturnsAsync(() => new AutoApproveViewModel());

        // Act
        var viewResult = await _pledgesController.AutoApproval(request) as ViewResult;
        var organisationNameViewModel = viewResult?.Model as AutoApproveViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            organisationNameViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_AutoApproval_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<AutoApprovePostRequest>();

        // Act
        var actionResult = await _pledgesController.AutoApproval(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_Sector_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<SectorRequest>();
        _orchestrator.Setup(x => x.GetSectorViewModel(request)).ReturnsAsync(() => new SectorViewModel());

        // Act
        var viewResult = await _pledgesController.Sector(request) as ViewResult;
        var amountViewModel = viewResult?.Model as SectorViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            amountViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_Sector_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<SectorPostRequest>();

        // Act
        var actionResult = await _pledgesController.Sector(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_Level_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<LevelRequest>();
        _orchestrator.Setup(x => x.GetLevelViewModel(request)).ReturnsAsync(() => new LevelViewModel());

        // Act
        var viewResult = await _pledgesController.Level(request) as ViewResult;
        var amountViewModel = viewResult?.Model as LevelViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            amountViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_Level_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<LevelViewModel>();

        // Act
        var actionResult = await _pledgesController.Level(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_JobRole_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<JobRoleRequest>();
        _orchestrator.Setup(x => x.GetJobRoleViewModel(request)).ReturnsAsync(() => new JobRoleViewModel());

        // Act
        var viewResult = await _pledgesController.JobRole(request) as ViewResult;
        var jobRoleViewModel = viewResult?.Model as JobRoleViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            jobRoleViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_JobRole_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<JobRolePostRequest>();

        // Act
        var actionResult = await _pledgesController.JobRole(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public async Task GET_Location_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<LocationRequest>();
        _orchestrator.Setup(x => x.GetLocationViewModel(request)).ReturnsAsync(() => new LocationViewModel());

        // Act
        var viewResult = await _pledgesController.Location(request) as ViewResult;
        var locationViewModel = viewResult?.Model as LocationViewModel;

        using (new AssertionScope())
        {
            // Assert
            viewResult.Should().NotBeNull();
            locationViewModel.Should().NotBeNull();
        }
    }

    [Test]
    public async Task POST_Location_Returns_Expected_Redirect_To_Create_With_Valid_Location()
    {
        // Arrange
        var request = _fixture.Create<LocationPostRequest>();
        _orchestrator
            .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(),
                It.IsAny<IDictionary<int, IEnumerable<string>>>()))
            .ReturnsAsync(new Dictionary<int, string>());

        // Act
        var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
            actionResult.RouteValues["cacheKey"].Should().Be(request.CacheKey);
        }
    }

    [Test]
    public async Task POST_Location_Returns_Expected_Redirect_To_Location_With_Invalid_Location()
    {
        // Arrange
        var request = _fixture.Create<LocationPostRequest>();
        _orchestrator
            .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(),
                It.IsAny<IDictionary<int, IEnumerable<string>>>()))
            .ReturnsAsync(new Dictionary<int, string> { { 1, "Error Message" } });

        // Act
        var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Location");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
            actionResult.RouteValues["cacheKey"].Should().Be(request.CacheKey);
        }
    }

    [Test]
    public async Task POST_Location_Returns_Expected_Redirect_To_LocationSelect()
    {
        // Arrange
        var request = _fixture
            .Build<LocationPostRequest>()
            .With(x => x.AllLocationsSelected, false)
            .Create();

        Action<LocationPostRequest, IDictionary<int, IEnumerable<string>>> validateCallback =
            (_, y) => { y.Add(_fixture.Create<KeyValuePair<int, IEnumerable<string>>>()); };

        _orchestrator
            .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(),
                It.IsAny<IDictionary<int, IEnumerable<string>>>()))
            .Callback(validateCallback)
            .ReturnsAsync(new Dictionary<int, string>());

        // Act
        var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("LocationSelect");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
            actionResult.RouteValues["cacheKey"].Should().Be(request.CacheKey);
        }
    }

    [Test]
    public async Task POST_LocationSelect_AllLocationsSelected_Redirect_To_Create()
    {
        // Arrange
        var request = _fixture
            .Build<LocationPostRequest>()
            .With(x => x.AllLocationsSelected, true)
            .Create();

        _orchestrator
            .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(),
                It.IsAny<IDictionary<int, IEnumerable<string>>>()))
            .ReturnsAsync(new Dictionary<int, string>());

        // Act
        var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Create");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
            actionResult.RouteValues["cacheKey"].Should().Be(request.CacheKey);
        }
    }

    [Test]
    public async Task GET_LocationSelect_Returns_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<LocationSelectRequest>();
        var expectedViewModel = _fixture.Create<LocationSelectViewModel>();

        _orchestrator
            .Setup(x => x.GetLocationSelectViewModel(It.IsAny<LocationSelectRequest>()))
            .ReturnsAsync(expectedViewModel);

        // Act
        var actionResult = await _pledgesController.LocationSelect(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var actualViewModel = model as LocationSelectViewModel;

        using (new AssertionScope())
        {
            // Assert
            actionResult.Should().NotBeNull();
            viewResult.Should().NotBeNull();
            model.Should().NotBeNull();
            actualViewModel.Should().NotBeNull();
            actualViewModel.Should().Be(expectedViewModel);
        }
    }

    [Test]
    public async Task POST_LocationSelect_Returns_View_With_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<LocationSelectPostRequest>();

        // Act
        var actionResult = await _pledgesController.LocationSelect(request);
        var redirectToAction = actionResult as RedirectToActionResult;

        using (new AssertionScope())
        {
            // Assert
            actionResult.Should().NotBeNull();
            redirectToAction.Should().NotBeNull();
            redirectToAction.ActionName.Should().Be(nameof(CreatePledgeController.Create));
        }
    }

    [Test]
    public async Task POST_Create_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<CreatePostRequest>();

        // Act
        var actionResult = await _pledgesController.Create(request) as RedirectToActionResult;

        // Assert
        using (new AssertionScope())
        {
            actionResult.Should().NotBeNull();
            actionResult.ActionName.Should().Be("Confirmation");
            actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        }
    }

    [Test]
    public void GET_Confirmation_Returns_Expected_View()
    {
        var request = _fixture.Create<ConfirmationRequest>();

        var viewResult = _pledgesController.Confirmation(request) as ViewResult;
        var viewmodel = viewResult?.Model as ConfirmationViewModel;

        using (new AssertionScope())
        {
            viewResult.Should().NotBeNull();
            viewmodel.Should().NotBeNull();
        }
    }
}