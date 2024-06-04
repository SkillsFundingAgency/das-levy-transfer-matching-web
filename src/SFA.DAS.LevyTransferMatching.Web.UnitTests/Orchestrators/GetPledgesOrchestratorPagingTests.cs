using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Services;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class GetPledgesOrchestratorPagingTests
{
    private PledgeOrchestrator _orchestrator;
    private Fixture _fixture;
    private Mock<IPledgeService> _pledgeService;
    private Infrastructure.Configuration.FeatureToggles _featureToggles;
    private GetPledgesResponse _pledgesResponse;
    private string _encodedAccountId;
    private readonly long _accountId = 1;
    private int _page = 1;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _pledgeService = new Mock<IPledgeService>();

        _featureToggles = new Infrastructure.Configuration.FeatureToggles();
        _pledgesResponse = _fixture.Create<GetPledgesResponse>();

        _pledgeService.Setup(x => x.GetPledges(_accountId, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_pledgesResponse);

        _orchestrator = new PledgeOrchestrator(_pledgeService.Object, Mock.Of<IEncodingService>(), Mock.Of<IUserService>(),
            _featureToggles,
            Mock.Of<IDateTimeService>(), Mock.Of<ICsvHelperService>());
    }

    [Test]
    public async Task GetPledgesViewModel_PagingData_Should_Be_Populated()
    {
        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        Assert.That(result.Paging.Page, Is.EqualTo(_pledgesResponse.Page));
        Assert.That(result.Paging.PageSize, Is.EqualTo(_pledgesResponse.PageSize));
        Assert.That(result.Paging.TotalPages, Is.EqualTo(_pledgesResponse.TotalPages));
        Assert.That(result.Paging.TotalPledges, Is.EqualTo(_pledgesResponse.TotalPledges));
    }

    [TestCase(3, 3, false)]
    [TestCase(50, 50, false)]
    [TestCase(51, 50, true)]
    public async Task GetPledgesViewModel_PagingData_Returns_Calculated_First_Page_Data_Correctly(int totalPledges, int endRow, bool expectedShowPageLinksStatus)
    {

        _pledgesResponse.Page = 1;
        _pledgesResponse.PageSize = 50;
        _pledgesResponse.TotalPledges = totalPledges;

        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        Assert.That(result.Paging.ShowPageLinks, Is.EqualTo(expectedShowPageLinksStatus));
        Assert.That(result.Paging.PageStartRow, Is.EqualTo(1));
        Assert.That(result.Paging.PageEndRow, Is.EqualTo(endRow));
    }

    [TestCase(2,150, 51, 100)]
    [TestCase(3, 151, 101, 150)]
    public async Task GetPledgesViewModel_PagingData_Returns_Calculated_Other_Page_Data_Correctly(int page, int totalPledges, int startRow, int endRow)
    {

        _pledgesResponse.Page = page;
        _pledgesResponse.PageSize = 50;
        _pledgesResponse.TotalPledges = totalPledges;

        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = page });
        Assert.That(result.Paging.ShowPageLinks, Is.True);
        Assert.That(result.Paging.PageStartRow, Is.EqualTo(startRow));
        Assert.That(result.Paging.PageEndRow, Is.EqualTo(endRow));
    }

    [TestCase(1, 10, false, false)]
    [TestCase(1, 1000, false, true)]
    [TestCase(10, 500, true, false)]
    public async Task GetPledgesViewModel_PagingData_Returns_Expected_Next_Previous_PageLinks(int page, int totalPledges, bool hasPreviousLink, bool hasNextLink)
    {
        _pledgesResponse.Page = page;
        _pledgesResponse.PageSize = 50;
        _pledgesResponse.TotalPledges = totalPledges;

        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = page });
        Assert.That(result.Paging.PageLinks.First().Label == "Previous", Is.EqualTo(hasPreviousLink));
        Assert.That(result.Paging.PageLinks.Last().Label == "Next", Is.EqualTo(hasNextLink));
    }

    [Test]
    public async Task GetPledgesViewModel_PagingData_Returns_Max_5_Numbered_PageLinks_With_Current_Page_Set_As_Current()
    {
        _pledgesResponse.Page = 10;
        _pledgesResponse.PageSize = 50;
        _pledgesResponse.TotalPledges = 10000;

        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x=>x.Label == "7"), Is.Null);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x=>x.Label == "8"), Is.Not.Null);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x=>x.Label == "9"), Is.Not.Null);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x=>x.Label == "10").IsCurrent, Is.True);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x => x.Label == "11"), Is.Not.Null);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x => x.Label == "12"), Is.Not.Null);
        Assert.That(result.Paging.PageLinks.FirstOrDefault(x => x.Label == "13"), Is.Null);
    }

    [Test]
    public async Task GetPledgesViewModel_PagingData_Returns_Page_number_for_query_string()
    {
        _pledgesResponse.Page = 10;
        _pledgesResponse.PageSize = 50;
        _pledgesResponse.TotalPledges = 10000;

        var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest
            { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        Assert.That(result.Paging.PageLinks.First().RouteData["page"], Is.EqualTo("9"));
        Assert.That(result.Paging.PageLinks.First(x => x.Label == "8").RouteData["page"], Is.EqualTo("8"));
        Assert.That(result.Paging.PageLinks.First(x => x.Label == "9").RouteData["page"], Is.EqualTo("9"));
        Assert.That(result.Paging.PageLinks.First(x => x.Label == "10").RouteData["page"], Is.EqualTo("10"));
        Assert.That(result.Paging.PageLinks.First(x => x.Label == "11").RouteData["page"], Is.EqualTo("11"));
        Assert.That(result.Paging.PageLinks.First(x => x.Label == "12").RouteData["page"], Is.EqualTo("12"));
        Assert.That(result.Paging.PageLinks.Last().RouteData["page"], Is.EqualTo("11"));
    }
}