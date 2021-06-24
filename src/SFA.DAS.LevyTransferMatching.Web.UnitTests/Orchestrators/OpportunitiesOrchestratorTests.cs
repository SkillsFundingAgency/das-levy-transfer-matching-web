using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using System;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System.Data;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class OpportunitiesOrchestratorTests
    {
        private OpportunitiesOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<IDateTimeService> _dateTimeService;
        private Mock<IOpportunitiesService> _opportunitiesService;
        private Mock<ITagService> _tagService;

        private List<OpportunityDto> _opportunityDtoList;
        private List<Tag> _sectorTags;
        private List<Tag> _jobRoleTags;
        private List<Tag> _levelTags;
        private DateTime _currentDateTime;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _dateTimeService = new Mock<IDateTimeService>();
            _opportunitiesService = new Mock<IOpportunitiesService>();
            _tagService = new Mock<ITagService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _opportunitiesService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);

            _orchestrator = new OpportunitiesOrchestrator(_dateTimeService.Object, _opportunitiesService.Object, _tagService.Object);
        }

        [Test]
        public async Task GetSearchFundingViewModel_Opportunities_Are_Populated()
        {
            var test = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(test.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(test.Opportunities[0].ReferenceNumber, _opportunityDtoList[0].EncodedPledgeId);
        }

        [Test]
        public async Task GetDetailViewModel_All_Selected_For_Everything_Tax_Year_Calculated_Successfully()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();

            this.SetupGetDetailViewModelServices();

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Sectors, _sectorTags.Select(y => y.TagId))
                .With(x => x.JobRoles, _jobRoleTags.Select(y => y.TagId))
                .With(x => x.Levels, _levelTags.Select(y => y.TagId))
                .Create();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync(opportunity);

            // Act
            var result = await _orchestrator.GetDetailViewModel(encodedId);

            // Assert
            Assert.AreEqual("All", result.JobRoleList);
            Assert.AreEqual("All", result.LevelList);
            Assert.AreEqual("All", result.SectorList);
            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");
        }

        [Test]
        public async Task GetDetailViewModel_Some_Selected_For_Everything_Tax_Year_Calculated_Successfully()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();

            this.SetupGetDetailViewModelServices();

            var sectors = _sectorTags.Take(4);
            var jobRoles = _jobRoleTags.Take(5);
            var levels = _levelTags.Take(6);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Sectors, sectors.Select(y => y.TagId))
                .With(x => x.JobRoles, jobRoles.Select(y => y.TagId))
                .With(x => x.Levels, levels.Select(y => y.TagId))
                .Create();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync(opportunity);

            // Act
            var result = await _orchestrator.GetDetailViewModel(encodedId);

            // Assert
            var jobRoleDescriptions = _jobRoleTags
                .Where(x => opportunity.JobRoles.Contains(x.TagId))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, string.Join(", ", jobRoleDescriptions));

            var levelDescriptions = levels.Select(x => x.TagId.Replace("Level", string.Empty));
            Assert.AreEqual(result.LevelList, string.Join(", ", levelDescriptions));

            var sectorDescriptions = _sectorTags
                .Where(x => opportunity.Sectors.Contains(x.TagId))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, string.Join(", ", sectorDescriptions));
            
            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");
        }

        [Test]
        public async Task GetDetailViewModel_One_Selected_For_Everything_Tax_Year_Calculated_Successfully()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();

            this.SetupGetDetailViewModelServices();

            var sectors = _sectorTags.Take(1);
            var jobRoles = _jobRoleTags.Take(1);
            var levels = _levelTags.Take(1);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Sectors, sectors.Select(y => y.TagId))
                .With(x => x.JobRoles, jobRoles.Select(y => y.TagId))
                .With(x => x.Levels, levels.Select(y => y.TagId))
                .Create();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync(opportunity);

            // Act
            var result = await _orchestrator.GetDetailViewModel(encodedId);

            // Assert
            var jobRoleDescriptions = _jobRoleTags
                .Where(x => opportunity.JobRoles.Contains(x.TagId))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, jobRoleDescriptions.Single());

            var levelDescriptions = levels.Select(x => x.TagId.Replace("Level", string.Empty));
            Assert.AreEqual(result.LevelList, levelDescriptions.Single());

            var sectorDescriptions = _sectorTags
                .Where(x => opportunity.Sectors.Contains(x.TagId))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, sectorDescriptions.Single());

            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");
        }

        [Test]
        public async Task GetDetailViewModel_Opportunity_Not_Found_Returns_Null()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync((OpportunityDto)null);

            // Act
            var result = await _orchestrator.GetDetailViewModel(encodedId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetDetailViewModel_Levels_Returned_Not_In_Correct_Format_Throws_Data_Exception()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();

            this.SetupGetDetailViewModelServices(correctlySetupLevels: false);

            var sectors = _sectorTags.Take(4);
            var jobRoles = _jobRoleTags.Take(5);
            var levels = _levelTags.Take(6);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Sectors, sectors.Select(y => y.TagId))
                .With(x => x.JobRoles, jobRoles.Select(y => y.TagId))
                .With(x => x.Levels, levels.Select(y => y.TagId))
                .Create();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<string>(y => y == encodedId)))
                .ReturnsAsync(opportunity);

            // Assert
            Assert.ThrowsAsync<DataException>(async () =>
            {
                // Act
                await _orchestrator.GetDetailViewModel(encodedId);
            });
        }

        private void SetupGetDetailViewModelServices(bool correctlySetupLevels = true)
        {
            _sectorTags = _fixture
                .CreateMany<Tag>(9)
                .ToList();
            _tagService
                .Setup(x => x.GetSectors())
                .ReturnsAsync(_sectorTags);

            _jobRoleTags = _fixture
                .CreateMany<Tag>(8)
                .ToList();
            _tagService
                .Setup(x => x.GetJobRoles())
                .ReturnsAsync(_jobRoleTags);

            if (correctlySetupLevels)
            {
                _levelTags = _fixture
                    .CreateMany<int>(7)
                    .Select(y =>
                    {
                        return _fixture
                            .Build<Tag>()
                            .With(z => z.TagId, $"Level{y}")
                            .Create();
                    })
                    .ToList();
            }
            else
            {
                _levelTags = _fixture
                    .CreateMany<Tag>(8)
                    .ToList();
            }

            _tagService
                .Setup(x => x.GetLevels())
                .ReturnsAsync(_levelTags);

            _currentDateTime = _fixture.Create<DateTime>();
            _dateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_currentDateTime);
        }
    }
}
