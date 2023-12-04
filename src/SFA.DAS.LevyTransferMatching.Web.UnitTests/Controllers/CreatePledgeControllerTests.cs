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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(indexViewModel, Is.Not.Null);
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(createViewModel, Is.Not.Null);
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(amountViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_Amount_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<AmountPostRequest>();

        // Act
        var actionResult = await _pledgesController.Amount(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(organisationNameViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_OrganisationName_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<OrganisationNamePostRequest>();

        // Act
        var actionResult = await _pledgesController.Organisation(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(organisationNameViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_AutoApproval_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<AutoApprovePostRequest>();

        // Act
        var actionResult = await _pledgesController.AutoApproval(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(amountViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_Sector_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<SectorPostRequest>();

        // Act
        var actionResult = await _pledgesController.Sector(request) as RedirectToActionResult;

        // Assert
       Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(amountViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_Level_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<LevelViewModel>();

        // Act
        var actionResult = await _pledgesController.Level(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(jobRoleViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_JobRole_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<JobRolePostRequest>();

        // Act
        var actionResult = await _pledgesController.JobRole(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(locationViewModel, Is.Not.Null);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
            Assert.That(actionResult.RouteValues["cacheKey"], Is.EqualTo(request.CacheKey));
        });
    }

    [Test]
    public async Task POST_Location_Returns_Expected_Redirect_To_Location_With_Invalid_Location()
    {
        // Arrange
        var request = _fixture.Create<LocationPostRequest>();
        _orchestrator
            .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(),
                It.IsAny<IDictionary<int, IEnumerable<string>>>()))
            .ReturnsAsync(new Dictionary<int, string>() { { 1, "Error Message" } });

        // Act
        var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Location"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
            Assert.That(actionResult.RouteValues["cacheKey"], Is.EqualTo(request.CacheKey));
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("LocationSelect"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
            Assert.That(actionResult.RouteValues["cacheKey"], Is.EqualTo(request.CacheKey));
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Create"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
            Assert.That(actionResult.RouteValues["cacheKey"], Is.EqualTo(request.CacheKey));
        });
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.EqualTo(expectedViewModel));
        });
    }

    [Test]
    public async Task POST_LocationSelect_Returns_View_With_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<LocationSelectPostRequest>();

        // Act
        var actionResult = await _pledgesController.LocationSelect(request);
        var redirectToAction = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(redirectToAction, Is.Not.Null);
            Assert.That(redirectToAction.ActionName, Is.EqualTo(nameof(CreatePledgeController.Create)));
        });
    }

    [Test]
    public async Task POST_Create_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<CreatePostRequest>();

        // Act
        var actionResult = await _pledgesController.Create(request) as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo("Confirmation"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
    }

    [Test]
    public void GET_Confirmation_Returns_Expected_View()
    {
        var request = _fixture.Create<ConfirmationRequest>();

        var viewResult = _pledgesController.Confirmation(request) as ViewResult;
        var viewmodel = viewResult?.Model as ConfirmationViewModel;

        Assert.Multiple(() =>
        {
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewmodel, Is.Not.Null);
        });
    }
}