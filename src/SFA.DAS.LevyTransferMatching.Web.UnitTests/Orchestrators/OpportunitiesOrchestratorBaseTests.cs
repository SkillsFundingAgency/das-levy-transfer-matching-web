using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using static SFA.DAS.LevyTransferMatching.Web.Orchestrators.OpportunitiesOrchestratorBase;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    public class OpportunitiesOrchestratorBaseTests
    {
        private Fixture _fixture;
        private TestOrchestrator _orchestrator;

        protected DateTime CurrentDateTime { get; set; }
        protected Mock<IDateTimeService> DateTimeService { get; private set; }

        protected List<ReferenceDataItem> SectorReferenceDataItems { get; private set; }
        protected List<ReferenceDataItem> JobRoleReferenceDataItems { get; private set; }
        protected List<ReferenceDataItem> LevelReferenceDataItems { get; private set; }

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            DateTimeService = new Mock<IDateTimeService>();

            _orchestrator = new TestOrchestrator(DateTimeService.Object);
        }

        [Test]
        public void GetOpportunitySummaryViewModel_One_Selected_For_Everything_Tax_Year_Calculated_Successfully()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

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
            var jobRoleDescriptions = JobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, jobRoleDescriptions.Single());

            var levelDescriptions = LevelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.AreEqual(result.LevelList, levelDescriptions.Single());

            var sectorDescriptions = SectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, sectorDescriptions.Single());

            Assert.AreEqual(result.YearDescription, $"{CurrentDateTime.ToTaxYear("yyyy")}/{CurrentDateTime.AddYears(1).ToTaxYear("yy")}");
        }

        [Test]
        public void GetOpportunitySummaryViewModel_All_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Is_Anonymous()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

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
            Assert.AreEqual("All", result.JobRoleList);
            Assert.AreEqual("All", result.LevelList);
            Assert.AreEqual("All", result.SectorList);
            Assert.AreEqual(result.YearDescription, $"{CurrentDateTime.ToTaxYear("yyyy")}/{CurrentDateTime.AddYears(1).ToTaxYear("yy")}");
            Assert.IsFalse(result.Description.Contains(encodedPledgeId));
        }

        [Test]
        public void GetOpportunitySummaryViewModel_Some_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Contains_EncodedPledgeId()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

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
            var jobRoleDescriptions = JobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, string.Join(", ", jobRoleDescriptions));

            var levelDescriptions = LevelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.AreEqual(result.LevelList, string.Join(", ", levelDescriptions));

            var sectorDescriptions = SectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, string.Join(", ", sectorDescriptions));

            Assert.AreEqual(result.YearDescription, $"{CurrentDateTime.ToTaxYear("yyyy")}/{CurrentDateTime.AddYears(1).ToTaxYear("yy")}");

            Assert.IsTrue(result.Description.Contains(encodedPledgeId));
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

            CurrentDateTime = _fixture.Create<DateTime>();
            DateTimeService
                .Setup(x => x.UtcNow)
                .Returns(CurrentDateTime);
        }

        private class TestOrchestrator : OpportunitiesOrchestratorBase
        {
            public TestOrchestrator(IDateTimeService dateTimeService) : base(dateTimeService)
            {
            }
        }
    }
}