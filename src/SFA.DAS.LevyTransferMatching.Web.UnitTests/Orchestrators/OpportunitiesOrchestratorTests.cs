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
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using ApplyRequest = SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types.ApplyRequest;


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
        private string _userId;
        private string _userDisplayName;

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
            _userId = _fixture.Create<string>();
            _userDisplayName = _fixture.Create<string>();

            _userService.Setup(x => x.GetUserId()).Returns(_userId);
            _userService.Setup(x => x.GetUserDisplayName()).Returns(_userDisplayName);

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

            GetContactDetailsResponse getContactDetailsResult = _fixture.Create<GetContactDetailsResponse>();

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

            GetContactDetailsResponse getContactDetailsResult = _fixture.Create<GetContactDetailsResponse>();

            _opportunitiesService
                .Setup(x => x.GetContactDetails(It.Is<long>(y => y == contactDetailsRequest.AccountId), It.Is<int>(y => y == contactDetailsRequest.PledgeId)))
                .ReturnsAsync(getContactDetailsResult);

            CreateApplicationCacheItem createApplicationCacheItem = _fixture.Create<CreateApplicationCacheItem>();

            _cacheStorageService
                .Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(It.Is<string>(y => y == contactDetailsRequest.CacheKey.ToString())))
                .ReturnsAsync(createApplicationCacheItem);

            var expectedEmailAddress = createApplicationCacheItem.EmailAddresses.First();
            var expectedAdditionalEmailAddresses = createApplicationCacheItem.EmailAddresses.Skip(1).Concat(new string[] { null, null });

            // Act
            ContactDetailsViewModel contactDetailsViewModel = await _orchestrator.GetContactDetailsViewModel(contactDetailsRequest);

            // Assert
            Assert.AreEqual(expectedEmailAddress, contactDetailsViewModel.EmailAddress);
            CollectionAssert.AreEqual(expectedAdditionalEmailAddresses, contactDetailsViewModel.AdditionalEmailAddresses);
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

        [Test]
        public async Task GetMoreDetailsViewModel_Is_Correct()
        {
            SetupGetOpportunityViewModelServices();

            var cacheKey = _fixture.Create<Guid>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
            var opportunityDto = _fixture.Create<OpportunityDto>();

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
            _encodingService.Setup(x => x.Decode(encodedPledgeId, EncodingType.PledgeId)).Returns(1);
            _opportunitiesService.Setup(x => x.GetOpportunity(1)).ReturnsAsync(opportunityDto);

            var result = await _orchestrator.GetMoreDetailsViewModel(new MoreDetailsRequest { EncodedAccountId = encodedAccountId, CacheKey = cacheKey, EncodedPledgeId = encodedPledgeId});

            Assert.IsNotNull(result);            
            Assert.AreEqual(cacheKey, result.CacheKey);
            Assert.AreEqual(encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(encodedPledgeId, result.EncodedPledgeId);
            Assert.AreEqual(cacheItem.Details, result.Details);
            Assert.IsNotNull(result.OpportunitySummaryViewModel);
            Assert.AreEqual(opportunityDto.Amount, result.OpportunitySummaryViewModel.Amount);
            Assert.AreEqual(string.Join(", ", opportunityDto.JobRoles.ToReferenceDataDescriptionList(_jobRoleReferenceDataItems)), result.OpportunitySummaryViewModel.JobRoleList);
            Assert.AreEqual(string.Join(", ", opportunityDto.Levels.ToReferenceDataDescriptionList(_levelReferenceDataItems, (x) => x.ShortDescription)), result.OpportunitySummaryViewModel.LevelList);
            Assert.AreEqual(string.Join(", ", opportunityDto.Sectors.ToReferenceDataDescriptionList(_sectorReferenceDataItems)), result.OpportunitySummaryViewModel.SectorList);
            Assert.AreEqual(_currentDateTime.ToTaxYearDescription(), result.OpportunitySummaryViewModel.YearDescription);

            _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
            _encodingService.Verify(x => x.Decode(encodedPledgeId, EncodingType.PledgeId), Times.Once);
            _opportunitiesService.Verify(x => x.GetOpportunity(1), Times.Once);
        }

        [Test]
        public async Task GetApplicationViewModel_Is_Correct()
        {
            SetupGetOpportunityViewModelServices();

            var cacheKey = _fixture.Create<Guid>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
            var applicationDetailsDto = _fixture.Create<ApplicationDetailsDto>();

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
            _opportunitiesService.Setup(x => x.GetApplicationDetails(1,1, default)).ReturnsAsync(applicationDetailsDto);

            var result = await _orchestrator.GetApplicationViewModel(new ApplicationDetailsRequest { EncodedAccountId = encodedAccountId, CacheKey = cacheKey, EncodedPledgeId = encodedPledgeId, PledgeId = 1, AccountId = 1  });

            Assert.IsNotNull(result);
            Assert.NotNull(result.SelectStandardViewModel);
            Assert.NotNull(result.SelectStandardViewModel.Standards);
            Assert.AreEqual(cacheKey, result.CacheKey);
            Assert.AreEqual(encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(encodedPledgeId, result.EncodedPledgeId);
            Assert.AreEqual(cacheItem.JobRole, result.JobRole);
            Assert.AreEqual(cacheItem.NumberOfApprentices, result.NumberOfApprentices);
            Assert.AreEqual(DateTime.Now.Year, result.MinYear);
            Assert.AreEqual(DateTime.Now.FinancialYearEnd().Year, result.MaxYear);
            Assert.AreEqual(cacheItem.HasTrainingProvider, result.HasTrainingProvider);
            Assert.IsNotNull(result.OpportunitySummaryViewModel);
            Assert.AreEqual(applicationDetailsDto.Opportunity.Amount, result.OpportunitySummaryViewModel.Amount);
            Assert.AreEqual(string.Join(", ", applicationDetailsDto.Opportunity.JobRoles.ToReferenceDataDescriptionList(_jobRoleReferenceDataItems)), result.OpportunitySummaryViewModel.JobRoleList);
            Assert.AreEqual(string.Join(", ", applicationDetailsDto.Opportunity.Levels.ToReferenceDataDescriptionList(_levelReferenceDataItems)), result.OpportunitySummaryViewModel.LevelList);
            Assert.AreEqual(string.Join(", ", applicationDetailsDto.Opportunity.Sectors.ToReferenceDataDescriptionList(_sectorReferenceDataItems)), result.OpportunitySummaryViewModel.SectorList);
            Assert.AreEqual(_currentDateTime.ToTaxYearDescription(), result.OpportunitySummaryViewModel.YearDescription);
            Assert.AreEqual(cacheItem.StartDate.Value.Month, result.Month);
            Assert.AreEqual(cacheItem.StartDate.Value.Year, result.Year);
            Assert.AreEqual(applicationDetailsDto.Standards.Count(), result.SelectStandardViewModel.Standards.Count());

            _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
            _opportunitiesService.Verify(x => x.GetApplicationDetails(1, 1, default), Times.Once);
        }

        [Test]
        public async Task GetApplyViewModel_Returns_Empty_Value_For_Null_HasTrainingProvider()
        {
            var applicationRequest = SetupForGetApplyViewModel();

            var orchestrator = new OpportunitiesOrchestrator(_dateTimeService.Object, _opportunitiesService.Object,
                _tagService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object);

            var result = await orchestrator.GetApplyViewModel(applicationRequest);

            Assert.AreEqual("-", result.HaveTrainingProvider);
        }

        [Test]
        public async Task GetApplyViewModel_Returns_Empty_Value_For_True_HasTrainingProvider()
        {
            var applicationRequest = SetupForGetApplyViewModel(true);
            var orchestrator = new OpportunitiesOrchestrator(_dateTimeService.Object, _opportunitiesService.Object,
                _tagService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object);

            var result = await orchestrator.GetApplyViewModel(applicationRequest);

            Assert.AreEqual("Yes", result.HaveTrainingProvider);
        }

        [Test]
        public async Task GetApplyViewModel_Returns_Empty_Value_For_False_HasTrainingProvider()
        {
            var applicationRequest = SetupForGetApplyViewModel(false);
            var orchestrator = new OpportunitiesOrchestrator(_dateTimeService.Object, _opportunitiesService.Object,
                _tagService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object); 

            var result = await orchestrator.GetApplyViewModel(applicationRequest);

            Assert.AreEqual("No", result.HaveTrainingProvider);
        }

        [Test]
        public async Task GetConfirmationViewModel_Returns_Expected_Model()
        {
            var encodedAccountId = _fixture.Create<string>();
            var accountId = _fixture.Create<long>();
            var opportunityId = _fixture.Create<int>();
            var response = _fixture.Create<GetConfirmationResponse>();
            var reference = _fixture.Create<string>();

            _opportunitiesService.Setup(x => x.GetConfirmation(accountId, opportunityId))
                .ReturnsAsync(response);

            _encodingService.Setup(x => x.Encode(opportunityId, EncodingType.PledgeId))
                .Returns(reference);

            var result =
                await _orchestrator.GetConfirmationViewModel(new ConfirmationRequest {PledgeId = opportunityId, AccountId = accountId, EncodedAccountId = encodedAccountId});

            Assert.AreEqual(response.AccountName, result.AccountName);
            Assert.AreEqual(response.IsNamePublic, result.IsNamePublic);
            Assert.AreEqual(reference, result.Reference);
            Assert.AreEqual(encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task SubmitApplication_Creates_Application()
        {
            var cacheKey = _fixture.Create<Guid>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var accountId = _fixture.Create<long>();
            var opportunityId = _fixture.Create<int>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()))
                .ReturnsAsync(cacheItem);

            var request = new ApplyPostRequest {CacheKey = cacheKey, EncodedAccountId = encodedAccountId, EncodedPledgeId = encodedPledgeId, AccountId = accountId, PledgeId = opportunityId};

            await _orchestrator.SubmitApplication(request);
            
             _opportunitiesService.Verify(x => x.PostApplication(accountId, opportunityId,
                 It.Is<ApplyRequest>(r => r.EncodedAccountId == encodedAccountId &&
                                          r.Details == cacheItem.Details &&
                                          r.StandardId == cacheItem.StandardId &&
                                          r.NumberOfApprentices == cacheItem.NumberOfApprentices.Value &&
                                          r.StartDate == cacheItem.StartDate &&
                                          r.HasTrainingProvider == cacheItem.HasTrainingProvider.Value &&
                                          r.Sectors.Equals(cacheItem.Sectors) &&
                                          r.Postcode == cacheItem.Postcode &&
                                          r.FirstName == cacheItem.FirstName &&
                                          r.LastName == cacheItem.LastName &&
                                          r.EmailAddresses == cacheItem.EmailAddresses &&
                                          r.BusinessWebsite == cacheItem.BusinessWebsite &&
                                          r.UserId == _userId &&
                                          r.UserDisplayName == _userDisplayName)));
        }

        [Test]
        public async Task SubmitApplication_Clears_CacheItem()
        {
            var cacheKey = _fixture.Create<Guid>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var accountId = _fixture.Create<long>();
            var opportunityId = _fixture.Create<int>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()))
                .ReturnsAsync(cacheItem);

            var request = new ApplyPostRequest { CacheKey = cacheKey, EncodedAccountId = encodedAccountId, EncodedPledgeId = encodedPledgeId, AccountId = accountId, PledgeId = opportunityId };

            await _orchestrator.SubmitApplication(request);

            _cacheStorageService.Verify(x => x.DeleteFromCache(cacheKey.ToString()), Times.Once);
        }

        private ApplicationRequest SetupForGetApplyViewModel(bool? hasTraininProvider = null)
        {
            var opportunity = _fixture.Create<OpportunityDto>();
            var applicationRequest = _fixture.Create<ApplicationRequest>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
            cacheItem.HasTrainingProvider = hasTraininProvider;

            var cacheKey = _fixture.Create<Guid>();
            applicationRequest.CacheKey = cacheKey;

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
            _opportunitiesService.Setup(x => x.GetOpportunity(applicationRequest.PledgeId)).ReturnsAsync(opportunity);
            _dateTimeService.SetupGet(x => x.UtcNow).Returns(DateTime.Now);

            return applicationRequest;
        }

        [Test]
        public async Task GetSectorViewModel_Is_Correct()
        {
            SetupGetOpportunityViewModelServices();

            var cacheKey = _fixture.Create<Guid>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedPledgeId = _fixture.Create<string>();
            var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
            var getSectorResponse = _fixture.Create<GetSectorResponse>();

            _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
            _opportunitiesService.Setup(x => x.GetSector(1, 1)).ReturnsAsync(getSectorResponse);

            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = encodedAccountId, CacheKey = cacheKey, EncodedPledgeId = encodedPledgeId, PledgeId = 1, AccountId = 1 });

            Assert.IsNotNull(result);
            Assert.AreEqual(cacheKey, result.CacheKey);
            Assert.AreEqual(encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(encodedPledgeId, result.EncodedPledgeId);
            Assert.AreEqual(cacheItem.Sectors, result.Sectors);
            Assert.AreEqual(cacheItem.Postcode, result.Postcode);
            Assert.IsNotNull(result.OpportunitySummaryViewModel);
            Assert.AreEqual(getSectorResponse.Opportunity.Amount, result.OpportunitySummaryViewModel.Amount);
            Assert.AreEqual(string.Join(", ", getSectorResponse.Opportunity.JobRoles.ToReferenceDataDescriptionList(_jobRoleReferenceDataItems)), result.OpportunitySummaryViewModel.JobRoleList);
            Assert.AreEqual(string.Join(", ", getSectorResponse.Opportunity.Levels.ToReferenceDataDescriptionList(_levelReferenceDataItems)), result.OpportunitySummaryViewModel.LevelList);
            Assert.AreEqual(string.Join(", ", getSectorResponse.Opportunity.Sectors.ToReferenceDataDescriptionList(_sectorReferenceDataItems)), result.OpportunitySummaryViewModel.SectorList);
            Assert.AreEqual(_currentDateTime.ToTaxYearDescription(), result.OpportunitySummaryViewModel.YearDescription);

            _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
            _opportunitiesService.Verify(x => x.GetSector(1, 1), Times.Once);
        }
    }
}