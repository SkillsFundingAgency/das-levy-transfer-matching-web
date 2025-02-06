using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class OpportunitiesOrchestratorPagingTests : OpportunitiesOrchestratorBaseTests
{
    private OpportunitiesOrchestrator _orchestrator;
    private Fixture _fixture;

    private Mock<IOpportunitiesService> _opportunitiesService;
    private Mock<IUserService> _userService;
    private Mock<IEncodingService> _encodingService;
    private Mock<ICacheStorageService> _cacheStorageService;

    private GetIndexResponse _getIndexResponse;
    private IndexRequest _indexRequest;
    private string _userId;
    private string _userDisplayName;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _opportunitiesService = new Mock<IOpportunitiesService>();
        _userService = new Mock<IUserService>();
        _encodingService = new Mock<IEncodingService>();
        _cacheStorageService = new Mock<ICacheStorageService>();

        _userId = _fixture.Create<string>();
        _userDisplayName = _fixture.Create<string>();

        _userService.Setup(x => x.GetUserId()).Returns(_userId);
        _userService.Setup(x => x.GetUserDisplayName()).Returns(_userDisplayName);

        _getIndexResponse = _fixture.Create<GetIndexResponse>();
        _indexRequest = _fixture.Create<IndexRequest>();
        _indexRequest.SortBy = OpportunitiesSortBy.ValueHighToLow.ToString();

        _opportunitiesService.Setup(x => x.GetIndex(_indexRequest.Sectors, OpportunitiesSortBy.ValueHighToLow, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_getIndexResponse);
        _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns("test");
        SetupGetOpportunityViewModelServices();

        _orchestrator = new OpportunitiesOrchestrator(DateTimeService.Object, _opportunitiesService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object);
    }

    [Test]
    public async Task GetIndexViewModel_PagingData_Is_Populated()
    {
        _indexRequest.Page = 1;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.Page.Should().Be(_getIndexResponse.Page);
        result.Paging.PageSize.Should().Be(_getIndexResponse.PageSize);
        result.Paging.TotalPages.Should().Be(_getIndexResponse.TotalPages);
        result.Paging.TotalItems.Should().Be(_getIndexResponse.TotalOpportunities);
    }

    [TestCase(3, 3, false)]
    [TestCase(50, 50, false)]
    [TestCase(51, 50, true)]
    public async Task GetIndexViewModel_PagingData_Returns_Calculated_First_Page_Data_Correctly(int totalOpportunities, int endRow, bool expectedShowPageLinksStatus)
    {

        _getIndexResponse.Page = 1;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = totalOpportunities;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.ShowPageLinks.Should().Be(expectedShowPageLinksStatus);
        result.Paging.PageStartRow.Should().Be(1);
        result.Paging.PageEndRow.Should().Be(endRow);
    }

    [TestCase(2, 150, 51, 100)]
    [TestCase(3, 151, 101, 150)]
    public async Task GetIndexViewModel_PagingData_Returns_Calculated_Other_Page_Data_Correctly(int page, int totalOpportunities, int startRow, int endRow)
    {
        _getIndexResponse.Page = page;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = totalOpportunities;
        _indexRequest.Page = page;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.ShowPageLinks.Should().BeTrue();
        result.Paging.PageStartRow.Should().Be(startRow);
        result.Paging.PageEndRow.Should().Be(endRow);
    }


    [TestCase(1, 10, false, false)]
    [TestCase(1, 1000, false, true)]
    [TestCase(10, 500, true, false)]
    public async Task GetIndexViewModel_PagingData_Returns_Expected_Next_Previous_PageLinks(int page, int totalOpportunities, bool hasPreviousLink, bool hasNextLink)
    {
        _getIndexResponse.Page = page;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = totalOpportunities;
        _indexRequest.Page = page;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        (result.Paging.PageLinks.First().Label == "Previous").Should().Be(hasPreviousLink);
        (result.Paging.PageLinks.Last().Label == "Next").Should().Be(hasNextLink);
    }


    [Test]
    public async Task GetIndexViewModel_PagingData_Returns_Max_7_Numbered_PageLinks_With_Current_Page_Set_As_Current()
    {
        _getIndexResponse.Page = 10;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = 10000;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "7").Should().BeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "8").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "9").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "10").IsCurrent.Should().BeTrue();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "11").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "12").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "13").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "14").Should().NotBeNull();
        result.Paging.PageLinks.FirstOrDefault(x => x.Label == "15").Should().BeNull();
    }

    [Test]
    public async Task GetIndexViewModel_PagingData_Returns_Page_number_for_query_string()
    {
        _getIndexResponse.Page = 10;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = 10000;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.PageLinks.First(x => x.Label == "8").RouteData["page"].Should().Be("8");
        result.Paging.PageLinks.First(x => x.Label == "9").RouteData["page"].Should().Be("9");
        result.Paging.PageLinks.First(x => x.Label == "10").RouteData["page"].Should().Be("10");
        result.Paging.PageLinks.First(x => x.Label == "11").RouteData["page"].Should().Be("11");
        result.Paging.PageLinks.First(x => x.Label == "12").RouteData["page"].Should().Be("12");
    }

    [Test]
    public async Task GetIndexViewModel_Previous_Next_Labels_Return_correct_page_numbers_for_query_string()
    {
        _getIndexResponse.Page = 10;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = 10000;

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.PageLinks.First().RouteData["page"].Should().Be("9");
        result.Paging.PageLinks.Last().RouteData["page"].Should().Be("11");
    }

    [Test]
    public async Task GetIndexViewModel_PageLinks_Contain_CommaSeparatedSectors_When_Passed_From_Request()
    {
        _getIndexResponse.Page = 10;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = 10000;

        _indexRequest.CommaSeparatedSectors = "Agriculture,Business,Charity";
        _indexRequest.Sectors = _indexRequest.GetSectorsList();

        _opportunitiesService.Setup(x => x.GetIndex(_indexRequest.Sectors, OpportunitiesSortBy.ValueHighToLow, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_getIndexResponse);

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.PageLinks.First().RouteData["CommaSeparatedSectors"].Should().BeEquivalentTo(_indexRequest.CommaSeparatedSectors);
    }

    [Test]
    public async Task GetIndexViewModel_PageLinks_Contain_SortBy_When_Passed_From_Request()
    {
        _getIndexResponse.Page = 10;
        _getIndexResponse.PageSize = 50;
        _getIndexResponse.TotalOpportunities = 10000;

        _opportunitiesService.Setup(x => x.GetIndex(_indexRequest.Sectors, OpportunitiesSortBy.ValueHighToLow, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_getIndexResponse);

        var result = await _orchestrator.GetIndexViewModel(_indexRequest);

        result.Paging.PageLinks.First().RouteData["SortBy"].Should().BeEquivalentTo(_indexRequest.SortBy);
    }
}