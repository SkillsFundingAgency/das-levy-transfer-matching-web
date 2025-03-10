﻿using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

public class CreatePledgeOrchestratorTests
{
    private CreatePledgeOrchestrator _orchestrator;
    private Fixture _fixture;
    private Mock<ICacheStorageService> _cache;
    private Mock<IPledgeService> _pledgeService;
    private Mock<IEncodingService> _encodingService;
    private Mock<ILocationValidatorService> _validatorService;
    private Mock<IUserService> _userService;
    private Mock<IDateTimeService> _dateTimeService;
    private List<ReferenceDataItem> _sectors;
    private List<ReferenceDataItem> _levels;
    private List<ReferenceDataItem> _jobRoles;
    private GetAmountResponse _amountResponse;
    private GetOrganisationNameResponse _organisationNameResponse;
    private GetSectorResponse _sectorResponse;
    private GetJobRoleResponse _jobRoleResponse;
    private GetLevelResponse _levelResponse;
    private GetPledgesResponse _pledgesResponse;
    private GetApplicationApprovedResponse _applicationApprovedResponse;
    private Infrastructure.Configuration.FeatureToggles _featureToggles;
    private string _encodedAccountId;
    private Guid _cacheKey;
    private readonly long _accountId = 1;
    private readonly int _pledgeId = 1;
    private string _encodedPledgeId;
    private readonly int _applicationId = 1;
    private string _userId;
    private string _userDisplayName;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _featureToggles = new Infrastructure.Configuration.FeatureToggles();
        _encodedAccountId = _fixture.Create<string>();
        _cacheKey = Guid.NewGuid();
        _cache = new Mock<ICacheStorageService>();
        _pledgeService = new Mock<IPledgeService>();
        _encodingService = new Mock<IEncodingService>();
        _validatorService = new Mock<ILocationValidatorService>();
        _userService = new Mock<IUserService>();
        _dateTimeService = new Mock<IDateTimeService>();
        _dateTimeService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);


        _sectors = _fixture.Create<List<ReferenceDataItem>>();
        _levels = _fixture.Create<List<ReferenceDataItem>>();
        _jobRoles = _fixture.Create<List<ReferenceDataItem>>();

        _amountResponse = _fixture.Create<GetAmountResponse>();
        _organisationNameResponse = _fixture.Create<GetOrganisationNameResponse>();
        _sectorResponse = new GetSectorResponse { Sectors = _sectors };
        _levelResponse = new GetLevelResponse { Levels = _levels };
        _jobRoleResponse = new GetJobRoleResponse { JobRoles = _jobRoles, Sectors = _sectors };
        _pledgesResponse = _fixture.Create<GetPledgesResponse>();
        _applicationApprovedResponse = _fixture.Create<GetApplicationApprovedResponse>();

        _encodedPledgeId = _fixture.Create<string>();

        _pledgeService.Setup(x => x.GetPledges(_accountId, 1, 50)).ReturnsAsync(_pledgesResponse);
        _pledgeService.Setup(x => x.GetCreate(_accountId)).ReturnsAsync(() => new GetCreateResponse
        {
            Sectors = _sectors,
            JobRoles = _jobRoles,
            Levels = _levels
        });

        _pledgeService.Setup(x => x.GetAmount(_encodedAccountId)).ReturnsAsync(_amountResponse);
        _pledgeService.Setup(x => x.GetOrganisationName(_encodedAccountId)).ReturnsAsync(_organisationNameResponse);
        _pledgeService.Setup(x => x.GetSector(_accountId)).ReturnsAsync(_sectorResponse);
        _pledgeService.Setup(x => x.GetJobRole(_accountId)).ReturnsAsync(_jobRoleResponse);
        _pledgeService.Setup(x => x.GetLevel(_accountId)).ReturnsAsync(_levelResponse);
        _pledgeService.Setup(x => x.GetApplicationApproved(_accountId, _pledgeId, _applicationId)).ReturnsAsync(_applicationApprovedResponse);

        _userId = _fixture.Create<string>();
        _userDisplayName = _fixture.Create<string>();
        //TODO these tests should be changed to properly test the IsUserChangeAuthorized and IsOwnerOrTransactor methods
        _userService.Setup(x => x.IsUserChangeAuthorized(It.IsAny<string>())).Returns(true);
        _userService.Setup(x => x.GetUserId()).Returns(_userId);
        _userService.Setup(x => x.GetUserDisplayName()).Returns(_userDisplayName);
        _userService.Setup(x => x.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

        _orchestrator = new CreatePledgeOrchestrator(_cache.Object, _pledgeService.Object, _encodingService.Object, _validatorService.Object, _userService.Object, _featureToggles);
    }

    [Test]
    public void GetIndexViewModel_EncodedId_Is_Correct()
    {
        var result = _orchestrator.GetInformViewModel(_encodedAccountId);
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public void GetIndexViewModel_CacheKey_Has_Value()
    {
        var result = _orchestrator.GetInformViewModel(_encodedAccountId);
        result.CacheKey.Should().NotBe(Guid.Empty);
    }


    [Test]
    public async Task GetCreateViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetCreateViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetCreateViewModel_Amount_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.Amount.Should().Be(cacheItem.Amount);
    }

    [Test]
    public async Task GetCreateViewModel_SectorOptions_Are_Populated()
    {
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.SectorOptions.Should().BeEquivalentTo(_sectors);
    }

    [Test]
    public async Task GetCreateViewModel_JobRoleOptions_Are_Populated()
    {
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.JobRoleOptions.Should().BeEquivalentTo(_jobRoles);
    }

    [Test]
    public async Task GetCreateViewModel_LevelOptions_Are_Populated()
    {
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.LevelOptions.Should().BeEquivalentTo(_levels);
    }

    [Test]
    public async Task GetCreateViewModel_IsNamePublic_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.IsNamePublic.Should().Be(cacheItem.IsNamePublic);
    }

    [Test]
    public async Task GetCreateViewModel_Sectors_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.Sectors, ["Business"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.Sectors.Should().BeEquivalentTo(cacheItem.Sectors);
    }

    [Test]
    public async Task GetCreateViewModel_JobRoles_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.JobRoles, ["Business"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.JobRoles.Should().BeEquivalentTo(cacheItem.JobRoles);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task GetCreateViewModel_AutoApprovalIsEnabled_Is_Correct(bool toggleValue)
    {
        _featureToggles.FeatureToggleApplicationAutoApprove = toggleValue;
        var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.AutoApprovalIsEnabled.Should().Be(toggleValue);
    }

    [Test]
    public async Task GetAmountViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetAmountViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetAmountViewModel_Amount_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.Amount.Should().Be(cacheItem.Amount.ToString());
    }

    [Test]
    public async Task GetOrganisationNameViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetOrganisationNameViewModel(new OrganisationNameRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetOrganisationNameViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetOrganisationNameViewModel(new OrganisationNameRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetOrganisationNameViewModel_IsNamePublic_Is_Retrieved_From_Cache()
    {
        var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetOrganisationNameViewModel(new OrganisationNameRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.IsNamePublic.Should().Be(cacheItem.IsNamePublic);
    }


    [Test]
    public async Task GetSectorViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetSectorViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetSectorViewModel_SectorOptions_Are_Populated()
    {
        var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.SectorOptions.Should().BeEquivalentTo(_sectors);
    }

    [Test]
    public async Task GetSectorViewModel_Sectors_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.Sectors, ["Agriculture"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.Sectors.Should().BeEquivalentTo(cacheItem.Sectors);
    }

    [Test]
    public async Task GetJobRoleViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetJobRoleViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetJobRoleViewModel_JobRoleOptions_Are_Populated()
    {
        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.JobRoleOptions.Should().BeEquivalentTo(_jobRoles);
    }

    [Test]
    public async Task GetJobRoleViewModel_JobRoles_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.JobRoles, ["Business"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.JobRoles.Should().BeEquivalentTo(cacheItem.JobRoles);
    }

    [Test]
    public async Task GetJobRoleViewModel_SectorOptions_Are_Populated()
    {
        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.SectorOptions.Should().BeEquivalentTo(_sectors);
    }

    [Test]
    public async Task GetJobRoleViewModel_Sectors_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.Sectors, ["Business"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.Sectors.Should().BeEquivalentTo(cacheItem.Sectors);
    }

    [Test]
    public async Task GetLevelViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetLevelViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetLevelViewModel_LevelOptions_Are_Populated()
    {
        var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.LevelOptions.Should().BeEquivalentTo(_levels);
    }

    [Test]
    public async Task GetLevelViewModel_Sectors_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.Levels, ["Level2"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.Levels.Should().BeEquivalentTo(cacheItem.Levels);
    }

    [Test]
    public async Task GetLocationViewModel_Locations_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .With(x => x.Locations, ["Manchester"])
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetLocationViewModel(new LocationRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.Locations.Should().BeEquivalentTo(cacheItem.Locations);
    }


    [Test]
    public async Task GetAutoApproveViewModel_EncodedId_Is_Correct()
    {
        var result = await _orchestrator.GetAutoApproveViewModel(new AutoApproveRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.EncodedAccountId.Should().Be(_encodedAccountId);
    }

    [Test]
    public async Task GetAutoApproveViewModel_CacheKey_Is_Correct()
    {
        var result = await _orchestrator.GetAutoApproveViewModel(new AutoApproveRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
        result.CacheKey.Should().Be(_cacheKey);
    }

    [Test]
    public async Task GetAutoApproveViewModel_AutoApprove_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

        var result = await _orchestrator.GetAutoApproveViewModel(new AutoApproveRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
        result.AutomaticApprovalOption.Should().Be(cacheItem.AutomaticApprovalOption);
    }

    [Test]
    public async Task SubmitPledge_Is_Correct()
    {
        var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
            .Create();

        _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);
        _cache.Setup(x => x.DeleteFromCache(_cacheKey.ToString()));
        _pledgeService.Setup(x => x.PostPledge(It.IsAny<CreatePledgeRequest>(), _accountId)).ReturnsAsync(_pledgeId);
        _encodingService.Setup(x => x.Encode(_pledgeId, EncodingType.PledgeId)).Returns(_encodedPledgeId);

        var result = await _orchestrator.CreatePledge(new CreatePostRequest { AccountId = _accountId, CacheKey = _cacheKey, EncodedAccountId = _encodedAccountId });

        _cache.Verify(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString()), Times.Once);
        _cache.Verify(x => x.DeleteFromCache(_cacheKey.ToString()), Times.Once);
        _pledgeService.Verify(x => x.PostPledge(It.IsAny<CreatePledgeRequest>(), _accountId), Times.Once);
        _encodingService.Verify(x => x.Encode(_pledgeId, EncodingType.PledgeId), Times.Once);

        result.Should().Be(_encodedPledgeId);
    }

    [Test]
    public async Task ValidateLocations_Returns_No_Errors_But_Caches_MultipleValidLocations()
    {
        // Arrange
        var request = _fixture.Create<LocationPostRequest>();
        var multipleValidLocations = new Dictionary<int, IEnumerable<string>>();
        IDictionary<int, IEnumerable<string>> cachedMultipleValidLocations = null;

        Action<LocationPostRequest, IDictionary<int, IEnumerable<string>>> validateLocationsCallback =
            (x, y) =>
            {
                var locations = _fixture.CreateMany<KeyValuePair<int, IEnumerable<string>>>();

                foreach (var location in locations)
                {
                    y.Add(location);
                }
            };
        
        _validatorService
            .Setup(x => x.ValidateLocations(It.Is<LocationPostRequest>(y => y == request), It.Is<IDictionary<int, IEnumerable<string>>>(y => y == multipleValidLocations)))
            .Callback(validateLocationsCallback)
            .ReturnsAsync(new Dictionary<int, string>());

        var cacheItem = _fixture
            .Build<LocationSelectionCacheItem>()
            .With(x => x.MultipleValidLocations, (IDictionary<int, IEnumerable<string>>)null)
            .Create();

        _cache
            .Setup(x => x.RetrieveFromCache<LocationSelectionCacheItem>(It.Is<string>(y => y == $"LocationSelectionCacheItem_{request.CacheKey}")))
            .ReturnsAsync(cacheItem);

        Action<string, LocationSelectionCacheItem, int> saveToCacheCallback =
            (x, y, z) => { cachedMultipleValidLocations = y.MultipleValidLocations; };
        _cache
            .Setup(x => x.SaveToCache(It.Is<string>(y => y == $"LocationSelectionCacheItem_{cacheItem.Key}"), It.Is<LocationSelectionCacheItem>(y => y == cacheItem), It.Is<int>(y => y == 1)))
            .Callback(saveToCacheCallback);

        // Act
        await _orchestrator.ValidateLocations(request, multipleValidLocations);

        // Assert
        cachedMultipleValidLocations.Should().BeEquivalentTo(multipleValidLocations);
    }

    [Test]
    public async Task ValidateLocations_Returns_Errors()
    {
        // Arrange
        var request = _fixture.Create<LocationPostRequest>();
        var multipleValidLocations = new Dictionary<int, IEnumerable<string>>();

        var errors = _fixture.Create<Dictionary<int, string>>();
        _validatorService
            .Setup(x => x.ValidateLocations(It.Is<LocationPostRequest>(y => y == request), It.Is<IDictionary<int, IEnumerable<string>>>(y => y == multipleValidLocations)))
            .ReturnsAsync(errors);

        // Act
        var result = await _orchestrator.ValidateLocations(request, multipleValidLocations);

        // Assert
        result.Should().BeEquivalentTo(errors);
    }

    [Test]
    public async Task GetLocationSelectViewModel_Returns_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<LocationSelectRequest>();

        var cacheItem = _fixture.Create<LocationSelectionCacheItem>();

        _cache
            .Setup(x => x.RetrieveFromCache<LocationSelectionCacheItem>(It.Is<string>(y => y == $"LocationSelectionCacheItem_{request.CacheKey}")))
            .ReturnsAsync(cacheItem);

        // Act
        var result = await _orchestrator.GetLocationSelectViewModel(request);

        // Assert
        foreach (var selectValidLocationGroup in result.SelectValidLocationGroups)
        {
            cacheItem.MultipleValidLocations.Keys.Should().Contain(selectValidLocationGroup.Index);

            var locationNames = selectValidLocationGroup.ValidLocationItems.Select(x => x.Value);

            locationNames.Should().BeEquivalentTo(cacheItem.MultipleValidLocations[selectValidLocationGroup.Index]);
        }
    }
}