using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class GetApplicationsOrchestratorPagingTests
{
    private ApplicationsOrchestrator _orchestrator;
    private Fixture _fixture;
    private Mock<IApplicationsService> _applicationsService;
    private Infrastructure.Configuration.FeatureToggles _featureToggles;
    private GetApplicationsResponse _applicationsResponse;
    private readonly string _encodedAccountId = "ABCD";
    private readonly long _accountId = 1;
    private readonly int _page = 1;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _applicationsService = new Mock<IApplicationsService>();

        _featureToggles = new Infrastructure.Configuration.FeatureToggles();
        _applicationsResponse = _fixture.Create<GetApplicationsResponse>();

        _applicationsService.Setup(x => x.GetApplications(
            It.Is<long>(x => x == _accountId), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()
            )).ReturnsAsync(_applicationsResponse);

        _orchestrator = new ApplicationsOrchestrator(_applicationsService.Object, Mock.Of<IEncodingService>(), _featureToggles, Mock.Of<IUserService>());
    }

    [Test]
    public async Task GetApplications_PagingData_Should_Be_Populated()
    {
        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Paging.Page.Should().Be(_applicationsResponse.Page);
        result.Paging.PageSize.Should().Be(_applicationsResponse.PageSize);
        result.Paging.TotalPages.Should().Be(_applicationsResponse.TotalPages);
        result.Paging.TotalItems.Should().Be(_applicationsResponse.TotalItems);
    }

    [TestCase(3, 3, false)]
    [TestCase(50, 50, false)]
    [TestCase(51, 50, true)]
    public async Task GetApplications_PagingData_Returns_Calculated_First_Page_Data_Correctly(int TotalItems, int endRow, bool expectedShowPageLinksStatus)
    {

        _applicationsResponse.Page = 1;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = TotalItems;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Paging.ShowPageLinks.Should().Be(expectedShowPageLinksStatus);
        result.Paging.PageStartRow.Should().Be(1);
        result.Paging.PageEndRow.Should().Be(endRow);
    }

    [TestCase(2, 150, 51, 100)]
    [TestCase(3, 151, 101, 150)]
    public async Task GetApplications_PagingData_Returns_Calculated_Other_Page_Data_Correctly(int page, int TotalItems, int startRow, int endRow)
    {

        _applicationsResponse.Page = page;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = TotalItems;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = page });
        result.Paging.ShowPageLinks.Should().BeTrue();
        result.Paging.PageStartRow.Should().Be(startRow);
        result.Paging.PageEndRow.Should().Be(endRow);
    }

    [TestCase(1, 10, false, false)]
    [TestCase(1, 1000, false, true)]
    [TestCase(10, 500, true, false)]
    public async Task GetApplications_PagingData_Returns_Expected_Next_Previous_PageLinks(int page, int TotalItems, bool hasPreviousLink, bool hasNextLink)
    {
        _applicationsResponse.Page = page;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = TotalItems;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = page });
        (result.Paging.PageLinks.First().Label == "Previous").Should().Be(hasPreviousLink);
        (result.Paging.PageLinks.Last().Label == "Next").Should().Be(hasNextLink);
    }

    [Test]
    public async Task GetApplications_PagingData_Returns_Max_5_Numbered_PageLinks_With_Current_Page_Set_As_Current()
    {
        _applicationsResponse.Page = 10;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = 10000;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "7").Should().BeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "8").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "9").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "10").IsCurrent.Should().BeTrue();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "11").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "12").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "13").Should().BeNull();
    }

    [Test]
    public async Task GetApplications_PagingData_Returns_Page_number_for_query_string()
    {
        _applicationsResponse.Page = 10;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = 10000;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Paging.PageLinks.First(x => x.Label == "8").RouteData["page"].Should().Be("8");
        result.Paging.PageLinks.First(x => x.Label == "9").RouteData["page"].Should().Be("9");
        result.Paging.PageLinks.First(x => x.Label == "10").RouteData["page"].Should().Be("10");
        result.Paging.PageLinks.First(x => x.Label == "11").RouteData["page"].Should().Be("11");
        result.Paging.PageLinks.First(x => x.Label == "12").RouteData["page"].Should().Be("12");
    }

    [Test]
    public async Task GetApplications_Previous_Next_Labels_Return_correct_page_numbers_for_query_string()
    {
        _applicationsResponse.Page = 10;
        _applicationsResponse.PageSize = 50;
        _applicationsResponse.TotalItems = 10000;

        var result = await _orchestrator.GetApplications(new GetApplicationsRequest
        { EncodedAccountId = _encodedAccountId, AccountId = _accountId, Page = _page });
        result.Paging.PageLinks.First().RouteData["page"].Should().Be("9");
        result.Paging.PageLinks.Last().RouteData["page"].Should().Be("11");
    }
}