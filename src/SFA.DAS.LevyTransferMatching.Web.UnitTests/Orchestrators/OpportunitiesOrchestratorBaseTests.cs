using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using static SFA.DAS.LevyTransferMatching.Web.Orchestrators.OpportunitiesOrchestratorBase;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

public class OpportunitiesOrchestratorBaseTests
{
    private Fixture _fixture;
    private TestOrchestrator _orchestrator;

    private DateTime _currentDateTime;
    protected Mock<IDateTimeService> DateTimeService { get; private set; }

    protected List<ReferenceDataItem> SectorReferenceDataItems { get; private set; }
    protected List<ReferenceDataItem> JobRoleReferenceDataItems { get; private set; }
    protected List<ReferenceDataItem> LevelReferenceDataItems { get; private set; }

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();

        DateTimeService = new Mock<IDateTimeService>();

        _orchestrator = new TestOrchestrator();
    }

    [Test]
    public void GetOpportunitySummaryViewModel_One_Selected_For_Everything_Tax_Year_Calculated_Successfully()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();

        SetupGetOpportunityViewModelServices();

        var sectors = SectorReferenceDataItems.Take(1);
        var jobRoles = JobRoleReferenceDataItems.Take(1);
        var levels = LevelReferenceDataItems.Take(1);

        var opportunity = _fixture
            .Build<OpportunityDto>()
            .With(x => x.Sectors, sectors.Select(y => y.Id))
            .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
            .With(x => x.Levels, levels.Select(y => y.Id))
            .Create();

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions()
        {
            Sectors = opportunity.Sectors,
            JobRoles = opportunity.JobRoles,
            Levels = opportunity.Levels,
            Locations = opportunity.Locations,
            AllSectors = SectorReferenceDataItems,
            AllJobRoles = JobRoleReferenceDataItems,
            AllLevels = LevelReferenceDataItems,
            Amount = opportunity.Amount,
            IsNamePublic = opportunity.IsNamePublic,
            DasAccountName = opportunity.DasAccountName,
            EncodedPledgeId = encodedPledgeId,
        };

        // Act
        var result = _orchestrator.GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions);

        // Assert
        Assert.Multiple(() =>
        {
            var jobRoleDescriptions = JobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.That(jobRoleDescriptions.Single(), Is.EqualTo(result.JobRoleList));

            var levelDescriptions = LevelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.That(levelDescriptions.Single(), Is.EqualTo(result.LevelList));

            var sectorDescriptions = SectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.That(sectorDescriptions.Single(), Is.EqualTo(result.SectorList));
        });

    }

    [Test]
    public void GetOpportunitySummaryViewModel_All_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Is_Anonymous()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();

        SetupGetOpportunityViewModelServices();

        var opportunity = _fixture
            .Build<OpportunityDto>()
            .With(x => x.IsNamePublic, false)
            .With(x => x.Sectors, SectorReferenceDataItems.Select(y => y.Id))
            .With(x => x.JobRoles, JobRoleReferenceDataItems.Select(y => y.Id))
            .With(x => x.Levels, LevelReferenceDataItems.Select(y => y.Id))
            .Create();

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions()
        {
            Sectors = opportunity.Sectors,
            JobRoles = opportunity.JobRoles,
            Levels = opportunity.Levels,
            Locations = opportunity.Locations,
            AllSectors = SectorReferenceDataItems,
            AllJobRoles = JobRoleReferenceDataItems,
            AllLevels = LevelReferenceDataItems,
            Amount = opportunity.Amount,
            IsNamePublic = opportunity.IsNamePublic,
            DasAccountName = opportunity.DasAccountName,
            EncodedPledgeId = encodedPledgeId,
        };

        // Act
        var result = _orchestrator.GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.JobRoleList, Is.EqualTo("All"));
            Assert.That(result.LevelList, Is.EqualTo("All"));
            Assert.That(result.SectorList, Is.EqualTo("All"));
            Assert.That(result.Description, Does.Not.Contain(encodedPledgeId));
        });
    }

    [Test]
    public void GetOpportunitySummaryViewModel_Some_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Contains_EncodedPledgeId()
    {
        // Arrange
        var encodedPledgeId = _fixture.Create<string>();

        SetupGetOpportunityViewModelServices();

        var sectors = SectorReferenceDataItems.Take(4);
        var jobRoles = JobRoleReferenceDataItems.Take(5);
        var levels = LevelReferenceDataItems.Take(6);

        var opportunity = _fixture
            .Build<OpportunityDto>()
            .With(x => x.IsNamePublic, true)
            .With(x => x.Sectors, sectors.Select(y => y.Id))
            .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
            .With(x => x.Levels, levels.Select(y => y.Id))
            .Create();

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions()
        {
            Sectors = opportunity.Sectors,
            JobRoles = opportunity.JobRoles,
            Levels = opportunity.Levels,
            Locations = opportunity.Locations,
            AllSectors = SectorReferenceDataItems,
            AllJobRoles = JobRoleReferenceDataItems,
            AllLevels = LevelReferenceDataItems,
            Amount = opportunity.Amount,
            IsNamePublic = opportunity.IsNamePublic,
            DasAccountName = opportunity.DasAccountName,
            EncodedPledgeId = encodedPledgeId,
        };

        // Act
        var result = _orchestrator.GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions);

        // Assert
        Assert.Multiple(() =>
        {
            var jobRoleDescriptions = JobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.That(string.Join("; ", jobRoleDescriptions), Is.EqualTo(result.JobRoleList));

            var levelDescriptions = LevelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.That(string.Join(", ", levelDescriptions), Is.EqualTo(result.LevelList));

            var sectorDescriptions = SectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.That(string.Join("; ", sectorDescriptions), Is.EqualTo(result.SectorList));


            Assert.That(result.Description, Does.Contain(encodedPledgeId));
        });
    }

    protected void SetupGetOpportunityViewModelServices()
    {
        SectorReferenceDataItems = _fixture
            .CreateMany<ReferenceDataItem>(9)
            .ToList();

        JobRoleReferenceDataItems = _fixture
            .CreateMany<ReferenceDataItem>(8)
            .ToList();

        LevelReferenceDataItems = _fixture
            .CreateMany<ReferenceDataItem>(7)
            .ToList();           

        _currentDateTime = _fixture.Create<DateTime>();
        DateTimeService
            .Setup(x => x.UtcNow)
            .Returns(_currentDateTime);
    }

    private class TestOrchestrator : OpportunitiesOrchestratorBase { }
}