﻿using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using ApplyRequest = SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types.ApplyRequest;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class OpportunitiesOrchestratorTests : OpportunitiesOrchestratorBaseTests
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
        _opportunitiesService.Setup(x => x.GetIndex(_indexRequest.Sectors)).ReturnsAsync(_getIndexResponse);
        _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns("test");

        _orchestrator = new OpportunitiesOrchestrator(DateTimeService.Object, _opportunitiesService.Object, _userService.Object, _encodingService.Object, _cacheStorageService.Object);
    }

    [Test]
    public async Task GetIndexViewModel_Opportunities_Are_Populated()
    {
        var encodedId = _fixture.Create<string>();
        _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PledgeId)).Returns(encodedId);

        var viewModel = await _orchestrator.GetIndexViewModel(_indexRequest);

        for (int i = 0; i < _getIndexResponse.Opportunities.Count; i++)
        {
            Assert.That(viewModel.Opportunities[i].EmployerName, Is.EqualTo(_getIndexResponse.Opportunities[i].IsNamePublic ? _getIndexResponse.Opportunities[i].DasAccountName : "Opportunity"));
        }
        Assert.That(viewModel.Opportunities.Select(x => x.Amount), Is.EqualTo(_getIndexResponse.Opportunities.Select(x => x.Amount)).AsCollection);
        Assert.That(viewModel.Opportunities[0].ReferenceNumber, Is.EqualTo(encodedId));
        Assert.That(viewModel.Opportunities.Select(x => x.Locations), Is.EqualTo(_getIndexResponse.Opportunities.Select(x => x.Locations.ToLocationsList())).AsCollection);
        Assert.That(viewModel.Opportunities.Select(x => x.Sectors), Is.EqualTo(_getIndexResponse.Opportunities.Select(x => x.Sectors.ToReferenceDataDescriptionList(_getIndexResponse.Sectors))).AsCollection);
        Assert.That(viewModel.Opportunities.Select(x => x.JobRoles), Is.EqualTo(_getIndexResponse.Opportunities.Select(x => x.JobRoles.ToReferenceDataDescriptionList(_getIndexResponse.JobRoles))).AsCollection);
        Assert.That(viewModel.Opportunities.Select(x => x.Levels), Is.EqualTo(_getIndexResponse.Opportunities.Select(x => x.Levels.ToReferenceDataDescriptionList(_getIndexResponse.Levels, descriptionSource: y => y.ShortDescription))).AsCollection);
    }

    [Test]
    public async Task GetDetailViewModel_Opportunity_Not_Found_Returns_Null()
    {
        // Arrange
        int id = _fixture.Create<int>();

        _opportunitiesService
            .Setup(x => x.GetDetail(It.Is<int>(y => y == id)))
            .ReturnsAsync(new GetDetailResponse());

        // Act
        var result = await _orchestrator.GetDetailViewModel(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetDetailViewModel_Opportunity_Found_Model_Populated()
    {
        // Arrange
        string encodedId = _fixture.Create<string>();
        int id = _fixture.Create<int>();

        this.SetupGetOpportunityViewModelServices();

        var sectors = SectorReferenceDataItems.Take(4);
        var jobRoles = JobRoleReferenceDataItems.Take(5);
        var levels = LevelReferenceDataItems.Take(6);

        var opportunity = _fixture
            .Build<GetDetailResponse.OpportunityData>()
            .With(x => x.Id, id)
            .With(x => x.Sectors, sectors.Select(y => y.Id))
            .With(x => x.JobRoles, jobRoles.Select(y => y.Id))
            .With(x => x.Levels, levels.Select(y => y.Id))
            .Create();

        var getDetailResponse = _fixture.Build<GetDetailResponse>()
            .With(x => x.Opportunity, opportunity)
            .Create();

        _encodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == id), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedId);

        _opportunitiesService
            .Setup(x => x.GetDetail(It.Is<int>(y => y == id)))
            .ReturnsAsync(getDetailResponse);

        // Act
        var result = await _orchestrator.GetDetailViewModel(id);

        // Assert
        Assert.That(result.OpportunitySummaryView, Is.Not.Null);
        Assert.That(result.EncodedPledgeId, Is.EqualTo(encodedId));
    }

    [Test]
    public async Task GetSelectAccountViewModel_UserHasVaryingAccountAccess_ReturnsFilteredAccounts()
    {
        // Arrange
        var userId = _fixture.Create<string>();
        _userService
            .Setup(x => x.GetUserId())
            .Returns(userId);

        // The list of accounts that the user has access to, across all
        // levels of access.
        var accountIds = _fixture.CreateMany<string>(3).ToArray();
            
        // The list of accounts that the user has access to with
        // Owner/Transactor privilages (a subset of the above)
        var userAccessAccountIds = accountIds.Take(2).ToArray();

        _userService
            .Setup(x => x.GetUserOwnerTransactorAccountIds())
            .Returns(userAccessAccountIds);


        var selectAccountRequest = _fixture.Create<SelectAccountRequest>();
        var getOpportunityApplyResponse = _fixture
            .Create<GetSelectAccountResponse>();
        getOpportunityApplyResponse.Accounts.First().EncodedAccountId = userAccessAccountIds.First();
        getOpportunityApplyResponse.Accounts.Last().EncodedAccountId = userAccessAccountIds.Last();

        _opportunitiesService
            .Setup(x => x.GetSelectAccount(It.Is<int>(y => y == selectAccountRequest.OpportunityId), It.Is<string>(y => y == userId)))
            .ReturnsAsync(getOpportunityApplyResponse);

        // Act
        var viewModel = await _orchestrator.GetSelectAccountViewModel(selectAccountRequest);

        // Assert
        Assert.That(viewModel.Accounts.Count(), Is.EqualTo(2));
        Assert.That(viewModel.EncodedOpportunityId, Is.EqualTo(selectAccountRequest.EncodedOpportunityId));
    }

    [Test]
    public async Task GetContactDetailsViewModel_No_Opportunity_Found_Returns_Null()
    {
        // Arrange
        ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

        _opportunitiesService
            .Setup(x => x.GetContactDetails(It.Is<long>(y => y == contactDetailsRequest.AccountId), It.Is<int>(y => y == contactDetailsRequest.PledgeId)))
            .ReturnsAsync((GetContactDetailsResponse)null);

        // Act
        ContactDetailsViewModel contactDetailsViewModel = await _orchestrator.GetContactDetailsViewModel(contactDetailsRequest);

        // Assert
        Assert.That(contactDetailsViewModel, Is.Null);
    }

    [Test]
    public async Task GetContactDetailsViewModel_Not_In_Cache_AdditionalEmailAddresses_Populated_Correctly()
    {
        // Arrange
        ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

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
        Assert.That(contactDetailsViewModel.AdditionalEmailAddresses, Is.EqualTo(expectedAdditionalEmailAddresses).AsCollection);
    }

    [Test]
    public async Task GetContactDetailsViewModel_Already_In_Cache_Result_Populated_Correctly()
    {
        // Arrange
        ContactDetailsRequest contactDetailsRequest = _fixture.Create<ContactDetailsRequest>();

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
        Assert.That(contactDetailsViewModel.EmailAddress, Is.EqualTo(expectedEmailAddress));
        Assert.That(contactDetailsViewModel.AdditionalEmailAddresses, Is.EqualTo(expectedAdditionalEmailAddresses).AsCollection);
    }

    [Test]
    public async Task GetMoreDetailsViewModel_Is_Correct()
    {
        SetupGetOpportunityViewModelServices();

        var cacheKey = _fixture.Create<Guid>();
        var encodedAccountId = _fixture.Create<string>();
        var encodedPledgeId = _fixture.Create<string>();
        var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
        var getMoreDetailsResponse = _fixture.Create<GetMoreDetailsResponse>();

        _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
        _opportunitiesService.Setup(x => x.GetMoreDetails(It.IsAny<long>(), It.IsAny<int>())).ReturnsAsync(getMoreDetailsResponse);

        var request = new MoreDetailsRequest { EncodedAccountId = encodedAccountId, CacheKey = cacheKey, EncodedPledgeId = encodedPledgeId };
        var result = await _orchestrator.GetMoreDetailsViewModel(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CacheKey, Is.EqualTo(cacheKey));
        Assert.That(result.EncodedAccountId, Is.EqualTo(encodedAccountId));
        Assert.That(result.EncodedPledgeId, Is.EqualTo(encodedPledgeId));
        Assert.That(result.Details, Is.EqualTo(cacheItem.Details));
        Assert.That(result.OpportunitySummaryViewModel, Is.Not.Null);
        Assert.That(result.OpportunitySummaryViewModel.Amount, Is.EqualTo(getMoreDetailsResponse.Opportunity.Amount));
        Assert.That(result.OpportunitySummaryViewModel.JobRoleList, Is.EqualTo(string.Join(", ", getMoreDetailsResponse.Opportunity.JobRoles.ToReferenceDataDescriptionList(getMoreDetailsResponse.JobRoles))));
        Assert.That(result.OpportunitySummaryViewModel.LevelList, Is.EqualTo(string.Join(", ", getMoreDetailsResponse.Opportunity.Levels.ToReferenceDataDescriptionList(getMoreDetailsResponse.Levels, (x) => x.ShortDescription))));
        Assert.That(result.OpportunitySummaryViewModel.SectorList, Is.EqualTo(string.Join(", ", getMoreDetailsResponse.Opportunity.Sectors.ToReferenceDataDescriptionList(getMoreDetailsResponse.Sectors))));

        _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
        _opportunitiesService.Verify(x => x.GetMoreDetails(request.AccountId, request.PledgeId), Times.Once);
    }

    [Test]
    public async Task GetApplicationViewModel_Is_Correct()
    {
        SetupGetOpportunityViewModelServices();

        var cacheKey = _fixture.Create<Guid>();
        var encodedAccountId = _fixture.Create<string>();
        var encodedPledgeId = _fixture.Create<string>();
        var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
        var applicationDetailsResponse = _fixture.Create<GetApplicationDetailsResponse>();

        _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
        _opportunitiesService.Setup(x => x.GetApplicationDetails(1,1, default)).ReturnsAsync(applicationDetailsResponse);

        var result = await _orchestrator.GetApplicationViewModel(new ApplicationDetailsRequest { EncodedAccountId = encodedAccountId, CacheKey = cacheKey, EncodedPledgeId = encodedPledgeId, PledgeId = 1, AccountId = 1  });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.SelectStandardViewModel, Is.Not.Null);
        Assert.That(result.SelectStandardViewModel.Standards, Is.Not.Null);
        Assert.That(result.CacheKey, Is.EqualTo(cacheKey));
        Assert.That(result.EncodedAccountId, Is.EqualTo(encodedAccountId));
        Assert.That(result.EncodedPledgeId, Is.EqualTo(encodedPledgeId));
        Assert.That(result.JobRole, Is.EqualTo(cacheItem.JobRole));
        Assert.That(result.NumberOfApprentices, Is.EqualTo(cacheItem.NumberOfApprentices));
        Assert.That(result.MinYear, Is.EqualTo(DateTime.Now.Year));
        Assert.That(result.MaxYear, Is.EqualTo(DateTime.Now.FinancialYearEnd().Year));
        Assert.That(result.HasTrainingProvider, Is.EqualTo(cacheItem.HasTrainingProvider));
        Assert.That(result.OpportunitySummaryViewModel, Is.Not.Null);
        Assert.That(result.OpportunitySummaryViewModel.Amount, Is.EqualTo(applicationDetailsResponse.Opportunity.RemainingAmount));
        Assert.That(applicationDetailsResponse.Opportunity.Sectors.ToReferenceDataDescriptionList(applicationDetailsResponse.Sectors), Is.EqualTo(result.OpportunitySummaryViewModel.SectorList));
        Assert.That(applicationDetailsResponse.Opportunity.JobRoles.ToReferenceDataDescriptionList(applicationDetailsResponse.JobRoles), Is.EqualTo(result.OpportunitySummaryViewModel.JobRoleList));
        Assert.That(applicationDetailsResponse.Opportunity.Levels.ToReferenceDataDescriptionList(applicationDetailsResponse.Levels, x => x.ShortDescription), Is.EqualTo(result.OpportunitySummaryViewModel.LevelList));
        Assert.That(result.Month, Is.EqualTo(cacheItem.StartDate.Value.Month));
        Assert.That(result.Year, Is.EqualTo(cacheItem.StartDate.Value.Year));
        Assert.That(result.SelectStandardViewModel.Standards.Count(), Is.EqualTo(applicationDetailsResponse.Standards.Count()));

        _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
        _opportunitiesService.Verify(x => x.GetApplicationDetails(1, 1, default), Times.Once);
    }

    [Test]
    public async Task GetApplyViewModel_Returns_Empty_Value_For_Null_HasTrainingProvider()
    {
        var applicationRequest = SetupForGetApplyViewModel();

        var orchestrator = new OpportunitiesOrchestrator(DateTimeService.Object, _opportunitiesService.Object,
            _userService.Object, _encodingService.Object, _cacheStorageService.Object);

        var result = await orchestrator.GetApplyViewModel(applicationRequest);

        Assert.That(result.HaveTrainingProvider, Is.EqualTo("-"));
    }

    [Test]
    public async Task GetApplyViewModel_Returns_Empty_Value_For_True_HasTrainingProvider()
    {
        var applicationRequest = SetupForGetApplyViewModel(true);
        var orchestrator = new OpportunitiesOrchestrator(DateTimeService.Object, _opportunitiesService.Object,
            _userService.Object, _encodingService.Object, _cacheStorageService.Object);

        var result = await orchestrator.GetApplyViewModel(applicationRequest);

        Assert.That(result.HaveTrainingProvider, Is.EqualTo("Yes"));
    }

    [Test]
    public async Task GetApplyViewModel_Returns_Empty_Value_For_False_HasTrainingProvider()
    {
        var applicationRequest = SetupForGetApplyViewModel(false);
        var orchestrator = new OpportunitiesOrchestrator(DateTimeService.Object, _opportunitiesService.Object,
            _userService.Object, _encodingService.Object, _cacheStorageService.Object); 

        var result = await orchestrator.GetApplyViewModel(applicationRequest);

        Assert.That(result.HaveTrainingProvider, Is.EqualTo("No"));
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

        Assert.That(result.AccountName, Is.EqualTo(response.AccountName));
        Assert.That(result.IsNamePublic, Is.EqualTo(response.IsNamePublic));
        Assert.That(result.Reference, Is.EqualTo(reference));
        Assert.That(result.EncodedAccountId, Is.EqualTo(encodedAccountId));
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
                                     r.Locations.Equals(cacheItem.Locations) &&
                                     r.AdditionalLocation == (cacheItem.AdditionalLocation ? cacheItem.AdditionLocationText : string.Empty) &&
                                     r.SpecificLocation == cacheItem.SpecificLocation &&
                                     r.FirstName == cacheItem.FirstName &&
                                     r.LastName == cacheItem.LastName &&
                                     r.EmailAddresses.Equals(cacheItem.EmailAddresses) &&
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
        var getApplyResponse = _fixture.Create<GetApplyResponse>();
        var applicationRequest = _fixture.Create<ApplicationRequest>();
        var cacheItem = _fixture.Create<CreateApplicationCacheItem>();
        cacheItem.HasTrainingProvider = hasTraininProvider;

        var cacheKey = _fixture.Create<Guid>();
        applicationRequest.CacheKey = cacheKey;

        _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString())).ReturnsAsync(cacheItem);
        _opportunitiesService.Setup(x => x.GetApply(applicationRequest.AccountId, applicationRequest.PledgeId)).ReturnsAsync(getApplyResponse);
        DateTimeService.SetupGet(x => x.UtcNow).Returns(DateTime.Now);

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

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CacheKey, Is.EqualTo(cacheKey));
        Assert.That(result.EncodedAccountId, Is.EqualTo(encodedAccountId));
        Assert.That(result.EncodedPledgeId, Is.EqualTo(encodedPledgeId));
        Assert.That(result.Sectors, Is.EqualTo(cacheItem.Sectors));

        Assert.That(result.OpportunitySummaryViewModel, Is.Not.Null);
        Assert.That(result.OpportunitySummaryViewModel.Amount, Is.EqualTo(getSectorResponse.Opportunity.Amount));
        Assert.That(getSectorResponse.Opportunity.Sectors.ToReferenceDataDescriptionList(getSectorResponse.Sectors), Is.EqualTo(result.OpportunitySummaryViewModel.SectorList));
        Assert.That(getSectorResponse.Opportunity.JobRoles.ToReferenceDataDescriptionList(getSectorResponse.JobRoles), Is.EqualTo(result.OpportunitySummaryViewModel.JobRoleList));
        Assert.That(getSectorResponse.Opportunity.Levels.ToReferenceDataDescriptionList(getSectorResponse.Levels, x => x.ShortDescription), Is.EqualTo(result.OpportunitySummaryViewModel.LevelList));

        _cacheStorageService.Verify(x => x.RetrieveFromCache<CreateApplicationCacheItem>(cacheKey.ToString()), Times.Once);
        _opportunitiesService.Verify(x => x.GetSector(1, 1), Times.Once);
    }
}