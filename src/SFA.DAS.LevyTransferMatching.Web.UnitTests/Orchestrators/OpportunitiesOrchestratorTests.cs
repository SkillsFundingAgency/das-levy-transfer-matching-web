using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using System;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System.Data;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Infrastructure.Models;

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
        private Mock<IUserService> _userService;
        private Mock<IEncodingService> _encodingService;
        private Mock<ICacheStorageService> _cacheStorageService;

        private List<ReferenceDataItem> _sectors;
        private List<ReferenceDataItem> _levels;
        private List<ReferenceDataItem> _jobRoles;

        private List<OpportunityDto> _opportunityDtoList;
        private List<ReferenceDataItem> _sectorReferenceDataItems;
        private List<ReferenceDataItem> _jobRoleReferenceDataItems;
        private List<ReferenceDataItem> _levelReferenceDataItems;

        private DateTime _currentDateTime;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _dateTimeService = new Mock<IDateTimeService>();
            _opportunitiesService = new Mock<IOpportunitiesService>();
            _tagService = new Mock<ITagService>();
            _userService = new Mock<IUserService>();
            _encodingService = new Mock<IEncodingService>();
            _cacheStorageService = new Mock<ICacheStorageService>();

            _opportunityDtoList = _fixture.Create<List<OpportunityDto>>();
            _sectors = _fixture.Create<List<ReferenceDataItem>>();
            _levels = _fixture.Create<List<ReferenceDataItem>>();
            _jobRoles = _fixture.Create<List<ReferenceDataItem>>();
            
            _opportunitiesService.Setup(x => x.GetAllOpportunities()).ReturnsAsync(_opportunityDtoList);
            _tagService.Setup(x => x.GetJobRoles()).ReturnsAsync(_jobRoles);
            _tagService.Setup(x => x.GetSectors()).ReturnsAsync(_sectors);
            _tagService.Setup(x => x.GetLevels()).ReturnsAsync(_levels);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns("test");

            _orchestrator = new OpportunitiesOrchestrator(_dateTimeService.Object, _opportunitiesService.Object, _tagService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object);
        }

        [Test]
        public async Task GetIndexViewModel_Opportunities_Are_Populated()
        {
            var viewModel = await _orchestrator.GetIndexViewModel();

            Assert.AreEqual(viewModel.Opportunities[0].EmployerName, _opportunityDtoList[0].DasAccountName);
            Assert.AreEqual(viewModel.Opportunities[0].Amount, _opportunityDtoList[0].Amount);
            Assert.AreEqual("test", viewModel.Opportunities[0].ReferenceNumber);
        }

        [Test]
        public async Task GetDetailViewModel_Opportunity_Not_Found_Returns_Null()
        {
            // Arrange
            int id = _fixture.Create<int>();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<int>(y => y == id)))
                .ReturnsAsync((OpportunityDto)null);

            // Act
            var result = await _orchestrator.GetDetailViewModel(id);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetDetailViewModel_Opportunity_Found_Model_Populated()
        {
            // Arrange
            string encodedId = _fixture.Create<string>();
            int id = _fixture.Create<int>();

            this.SetupGetOpportunityViewModelServices();

            var sectors = _sectorReferenceDataItems.Take(4);
            var jobRoles = _jobRoleReferenceDataItems.Take(5);
            var levels = _levelReferenceDataItems.Take(6);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Id, id)
                .With(x => x.Sectors, sectors.Select(y => y.Id))
                .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
                .With(x => x.Levels, levels.Select(y => y.Id))
                .Create();

            _encodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == id), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedId);

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<int>(y => y == id)))
                .ReturnsAsync(opportunity);

            // Act
            var result = await _orchestrator.GetDetailViewModel(id);

            // Assert
            Assert.IsNotNull(result.OpportunitySummaryView);
            Assert.AreEqual(encodedId, result.EncodedPledgeId);
        }

        [Test]
        public async Task GetOpportunitySummaryViewModel_All_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Is_Anonymous()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.IsNamePublic, false)
                .With(x => x.Sectors, _sectorReferenceDataItems.Select(y => y.Id))
                .With(x => x.JobRoles, _jobRoleReferenceDataItems.Select(y => y.Id))
                .With(x => x.Levels, _levelReferenceDataItems.Select(y => y.Id))
                .Create();

            // Act
            var result = await _orchestrator.GetOpportunitySummaryViewModel(opportunity, encodedPledgeId);

            // Assert
            Assert.AreEqual("All", result.JobRoleList);
            Assert.AreEqual("All", result.LevelList);
            Assert.AreEqual("All", result.SectorList);
            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");
            Assert.IsFalse(result.Description.Contains(encodedPledgeId));
        }

        [Test]
        public async Task GetOpportunitySummaryViewModel_Some_Selected_For_Everything_Tax_Year_Calculated_Successfully_And_Description_Contains_EncodedPledgeId()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

            var sectors = _sectorReferenceDataItems.Take(4);
            var jobRoles = _jobRoleReferenceDataItems.Take(5);
            var levels = _levelReferenceDataItems.Take(6);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.IsNamePublic, true)
                .With(x => x.Sectors, sectors.Select(y => y.Id))
                .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
                .With(x => x.Levels, levels.Select(y => y.Id))
                .Create();

            // Act
            var result = await _orchestrator.GetOpportunitySummaryViewModel(opportunity, encodedPledgeId);

            // Assert
            var jobRoleDescriptions = _jobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, string.Join(", ", jobRoleDescriptions));

            var levelDescriptions = _levelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.AreEqual(result.LevelList, string.Join(", ", levelDescriptions));

            var sectorDescriptions = _sectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, string.Join(", ", sectorDescriptions));
            
            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");

            Assert.IsTrue(result.Description.Contains(encodedPledgeId));
        }

        [Test]
        public async Task GetOpportunitySummaryViewModel_One_Selected_For_Everything_Tax_Year_Calculated_Successfully()
        {
            // Arrange
            string encodedPledgeId = _fixture.Create<string>();

            this.SetupGetOpportunityViewModelServices();

            var sectors = _sectorReferenceDataItems.Take(1);
            var jobRoles = _jobRoleReferenceDataItems.Take(1);
            var levels = _levelReferenceDataItems.Take(1);

            var opportunity = _fixture
                .Build<OpportunityDto>()
                .With(x => x.Sectors, sectors.Select(y => y.Id))
                .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
                .With(x => x.Levels, levels.Select(y => y.Id))
                .Create();

            // Act
            var result = await _orchestrator.GetOpportunitySummaryViewModel(opportunity, encodedPledgeId);

            // Assert
            var jobRoleDescriptions = _jobRoleReferenceDataItems
                .Where(x => opportunity.JobRoles.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.JobRoleList, jobRoleDescriptions.Single());

            var levelDescriptions = _levelReferenceDataItems
                .Where(x => opportunity.Levels.Contains(x.Id))
                .Select(x => x.ShortDescription);
            Assert.AreEqual(result.LevelList, levelDescriptions.Single());

            var sectorDescriptions = _sectorReferenceDataItems
                .Where(x => opportunity.Sectors.Contains(x.Id))
                .Select(x => x.Description);
            Assert.AreEqual(result.SectorList, sectorDescriptions.Single());

            Assert.AreEqual(result.YearDescription, $"{_currentDateTime.ToTaxYear("yyyy")}/{_currentDateTime.AddYears(1).ToTaxYear("yy")}");
        }

        [Test]
        public async Task GetContactDetailsViewModel_No_Opportunity_Found_Returns_Null()
        {
            // Arrange
            ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

            _opportunitiesService
                .Setup(x => x.GetOpportunity(It.Is<int>(y => y == contactDetailsRequest.PledgeId)))
                .ReturnsAsync((OpportunityDto)null);

            // Act
            ContactDetailsViewModel contactDetailsViewModel = await _orchestrator.GetContactDetailsViewModel(contactDetailsRequest);

            // Assert
            Assert.IsNull(contactDetailsViewModel);
        }

        [Test]
        public async Task GetContactDetailsViewModel_Not_In_Cache_AdditionalEmailAddresses_Populated_Correctly()
        {
            // Arrange
            ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

            _currentDateTime = _fixture.Create<DateTime>();
            _dateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_currentDateTime);

            GetContactDetailsResult getContactDetailsResult = _fixture.Create<GetContactDetailsResult>();

            _opportunitiesService
                .Setup(x => x.GetContactDetails(It.Is<long>(y => y == contactDetailsRequest.AccountId), It.Is<int>(y => y == contactDetailsRequest.PledgeId)))
                .ReturnsAsync(getContactDetailsResult);

            string[] expectedAdditionalEmailAddresses = new string[] { null, null, null, null, };

            _cacheStorageService
                .Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(It.Is<string>(y => y == contactDetailsRequest.CacheKey.ToString())))
                .ReturnsAsync((CreateApplicationCacheItem)null);

            // Act
            ContactDetailsViewModel contactDetailsViewModel = await _orchestrator.GetContactDetailsViewModel(contactDetailsRequest);

            // Assert
            CollectionAssert.AreEqual(expectedAdditionalEmailAddresses, contactDetailsViewModel.AdditionalEmailAddresses);
        }

        [Test]
        public async Task GetContactDetailsViewModel_Already_In_Cache_Result_Populated_Correctly()
        {
            // Arrange
            ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

            _currentDateTime = _fixture.Create<DateTime>();
            _dateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_currentDateTime);

            GetContactDetailsResult getContactDetailsResult = _fixture.Create<GetContactDetailsResult>();

            _opportunitiesService
                .Setup(x => x.GetContactDetails(It.Is<long>(y => y == contactDetailsRequest.AccountId), It.Is<int>(y => y == contactDetailsRequest.PledgeId)))
                .ReturnsAsync(getContactDetailsResult);

            CreateApplicationCacheItem createApplicationCacheItem = _fixture.Create<CreateApplicationCacheItem>();

            _cacheStorageService
                .Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(It.Is<string>(y => y == contactDetailsRequest.CacheKey.ToString())))
                .ReturnsAsync(createApplicationCacheItem);

            // Act
            ContactDetailsViewModel contactDetailsViewModel = await _orchestrator.GetContactDetailsViewModel(contactDetailsRequest);

            // Assert
            CollectionAssert.AreEqual(createApplicationCacheItem.AdditionalEmailAddresses, contactDetailsViewModel.AdditionalEmailAddresses);
        }

        private void SetupGetOpportunityViewModelServices()
        {
            _sectorReferenceDataItems = _fixture
                .CreateMany<ReferenceDataItem>(9)
                .ToList();
            _tagService
                .Setup(x => x.GetSectors())
                .ReturnsAsync(_sectorReferenceDataItems);

            _jobRoleReferenceDataItems = _fixture
                .CreateMany<ReferenceDataItem>(8)
                .ToList();
            _tagService
                .Setup(x => x.GetJobRoles())
                .ReturnsAsync(_jobRoleReferenceDataItems);

            _levelReferenceDataItems = _fixture
                .CreateMany<ReferenceDataItem>(7)
                .ToList();

            _tagService
                .Setup(x => x.GetLevels())
                .ReturnsAsync(_levelReferenceDataItems);

            _currentDateTime = _fixture.Create<DateTime>();
            _dateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_currentDateTime);
        }
    }
}