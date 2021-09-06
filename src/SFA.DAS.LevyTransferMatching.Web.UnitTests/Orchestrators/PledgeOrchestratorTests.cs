using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
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
using ApplicationRequest = SFA.DAS.LevyTransferMatching.Web.Models.Pledges.ApplicationRequest;
using GetApplicationsResponse = SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types.GetApplicationsResponse;
using SectorRequest = SFA.DAS.LevyTransferMatching.Web.Models.Pledges.SectorRequest;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class PledgeOrchestratorTests
    {
        private PledgeOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<ICacheStorageService> _cache;
        private Mock<IPledgeService> _pledgeService;
		private Mock<IEncodingService> _encodingService;
        private Mock<ILocationValidatorService> _validatorService;
        private Mock<IUserService> _userService;
        private Mock<IDateTimeService> _dateTimeService;
        private Infrastructure.Configuration.FeatureToggles _featureToggles;
        private List<ReferenceDataItem> _sectors;
        private List<ReferenceDataItem> _levels;
        private List<ReferenceDataItem> _jobRoles;
        private GetAmountResponse _amountResponse;
        private GetSectorResponse _sectorResponse;
        private GetJobRoleResponse _jobRoleResponse;
        private GetLevelResponse _levelResponse;
        private GetPledgesResponse _pledgesResponse;
        private string _encodedAccountId;
        private Guid _cacheKey;
        private readonly long _accountId = 1;
        private readonly int _pledgeId = 1;
        private string _encodedPledgeId;
        private string _userId;
        private string _userDisplayName;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _encodedAccountId = _fixture.Create<string>();
            _cacheKey = Guid.NewGuid();
            _cache = new Mock<ICacheStorageService>();
            _pledgeService = new Mock<IPledgeService>();
            _encodingService = new Mock<IEncodingService>();
            _validatorService = new Mock<ILocationValidatorService>();
            _userService = new Mock<IUserService>();
            _dateTimeService = new Mock<IDateTimeService>();
            _dateTimeService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            _featureToggles = new Infrastructure.Configuration.FeatureToggles();

            _sectors = _fixture.Create<List<ReferenceDataItem>>();
            _levels = _fixture.Create<List<ReferenceDataItem>>();
            _jobRoles = _fixture.Create<List<ReferenceDataItem>>();

            _amountResponse = _fixture.Create<GetAmountResponse>();
            _sectorResponse = new GetSectorResponse {Sectors = _sectors};
            _levelResponse = new GetLevelResponse {Levels = _levels};
            _jobRoleResponse = new GetJobRoleResponse {JobRoles = _jobRoles};
            _pledgesResponse = _fixture.Create<GetPledgesResponse>();
           
            _encodedPledgeId = _fixture.Create<string>();
            _userId = _fixture.Create<string>();
            _userDisplayName = _fixture.Create<string>();

            _pledgeService.Setup(x => x.GetPledges(_accountId)).ReturnsAsync(_pledgesResponse);
            _pledgeService.Setup(x => x.GetCreate(_accountId)).ReturnsAsync(() => new GetCreateResponse
            {
                Sectors = _sectors,
                JobRoles = _jobRoles,
                Levels = _levels
            });

            _pledgeService.Setup(x => x.GetAmount(_encodedAccountId)).ReturnsAsync(_amountResponse);
            _pledgeService.Setup(x => x.GetSector(_accountId)).ReturnsAsync(_sectorResponse);
            _pledgeService.Setup(x => x.GetJobRole(_accountId)).ReturnsAsync(_jobRoleResponse);
            _pledgeService.Setup(x => x.GetLevel(_accountId)).ReturnsAsync(_levelResponse);

            _userService.Setup(x => x.IsUserChangeAuthorized()).Returns(true);

            _orchestrator = new PledgeOrchestrator(_cache.Object, _pledgeService.Object, _encodingService.Object, _validatorService.Object, _userService.Object, _featureToggles, _dateTimeService.Object);
        }

        [Test]
        public void GetIndexViewModel_EncodedId_Is_Correct()
        {
            var result = _orchestrator.GetInformViewModel(_encodedAccountId);
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public void GetIndexViewModel_CacheKey_Has_Value()
        {
            var result = _orchestrator.GetInformViewModel(_encodedAccountId);
            Assert.AreNotEqual(Guid.Empty, result.CacheKey);
        }

        [Test]
        public async Task GetPledgesViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetPledgesViewModel_RenderCreatePledgeButton_Is_True_When_Authorized()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.IsTrue(result.RenderCreatePledgeButton);
        }

        [Test]
        public async Task GetPledgesViewModel_Pledges_Is_Populated()
        {
            var result = await _orchestrator.GetPledgesViewModel(new PledgesRequest { EncodedAccountId = _encodedAccountId, AccountId = _accountId });
            Assert.NotNull(result.Pledges);
        }

        [Test]
        public async Task GetCreateViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest{ EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId});
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetCreateViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId});
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetCreateViewModel_Amount_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.Amount, result.Amount);
        }

        [Test]
        public async Task GetCreateViewModel_SectorOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_sectors, result.SectorOptions);
        }

        [Test]
        public async Task GetCreateViewModel_JobRoleOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_jobRoles, result.JobRoleOptions);
        }

        [Test]
        public async Task GetCreateViewModel_LevelOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_levels, result.LevelOptions);
        }

        [Test]
        public async Task GetCreateViewModel_IsNamePublic_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.IsNamePublic, result.IsNamePublic);
        }

        [Test]
        public async Task GetCreateViewModel_Sectors_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                .With(x => x.Sectors, new List<string>{"Business"})
                .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.Sectors, result.Sectors);
        }

        [Test]
        public async Task GetCreateViewModel_JobRoles_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                .With(x => x.JobRoles, new List<string>{ "Business"})
                .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.JobRoles, result.JobRoles);
        }

        [Test]
        public async Task GetAmountViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetAmountViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetAmountViewModel_Amount_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Amount.ToString(), result.Amount);
        }

        [Test]
        public async Task GetAmountViewModel_IsNamePublic_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetAmountViewModel(new AmountRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.IsNamePublic, result.IsNamePublic);
        }

        [Test]
        public async Task GetSectorViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetSectorViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetSectorViewModel_SectorOptions_Are_Populated()
        {
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_sectors, result.SectorOptions);
        }

        [Test]
        public async Task GetSectorViewModel_Sectors_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.Sectors, new List<string> { "Agriculture" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.Sectors, result.Sectors);
        }

        [Test]
        public async Task GetJobRoleViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetJobRoleViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetJobRoleViewModel_JobRoleOptions_Are_Populated()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_jobRoles, result.JobRoleOptions);
        }

        [Test]
        public async Task GetJobRoleViewModel_JobRoles_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.JobRoles, new List<string> { "Business" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.JobRoles, result.JobRoles);
        }

        [Test]
        public async Task GetLevelViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetLevelViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetLevelViewModel_LevelOptions_Are_Populated()
        { 
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(_levels, result.LevelOptions);
        }

        [Test]
        public async Task GetLevelViewModel_Sectors_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.Levels, new List<string> { "Level2" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey, AccountId = _accountId });
            Assert.AreEqual(cacheItem.Levels, result.Levels);
        }

        [Test]
        public async Task GetLocationViewModel_Locations_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.Locations, new List<string> { "Manchester" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetLocationViewModel(new LocationRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Locations, result.Locations);
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

            var result = await _orchestrator.SubmitPledge(new CreatePostRequest { AccountId = _accountId, CacheKey = _cacheKey, EncodedAccountId = _encodedAccountId });

            _cache.Verify(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString()), Times.Once);
            _cache.Verify(x => x.DeleteFromCache(_cacheKey.ToString()), Times.Once);
            _pledgeService.Verify(x => x.PostPledge(It.IsAny<CreatePledgeRequest>(), _accountId), Times.Once);
            _encodingService.Verify(x => x.Encode(_pledgeId, EncodingType.PledgeId), Times.Once);

            Assert.AreEqual(_encodedPledgeId, result);
        }

        [Test]
        public async Task GetApplications_Returns_Valid_ViewModel()
        {
            var response = new GetApplicationsResponse()
            {
                Applications = new List<GetApplicationsResponse.Application>()
                {
                    new GetApplicationsResponse.Application()
                    {
                        Id = 0,
                        StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                        Standard = _fixture.Create<StandardsListItemDto>()
                    }
                }
            };

            response.Applications.First().Standard.ApprenticeshipFunding = new List<ApprenticeshipFundingDto>()
            {
                new ApprenticeshipFundingDto()
                {
                    Duration = 12,
                    EffectiveFrom = new DateTime(DateTime.UtcNow.AddYears(-1).Year, DateTime.UtcNow.Month, 1),
                    EffectiveTo = new DateTime(DateTime.UtcNow.AddYears(1).Year, DateTime.UtcNow.Month, 1),
                    MaxEmployerLevyCap = 100_000
                }
            };

            _pledgeService.Setup(x => x.GetApplications(0, 0)).ReturnsAsync(response);
            _encodingService.Setup(x => x.Encode(0, EncodingType.PledgeApplicationId)).Returns("123");

            var result = await _orchestrator.GetApplications(new ApplicationsRequest() { EncodedAccountId = _encodedAccountId, EncodedPledgeId = _encodedPledgeId});

            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
            Assert.AreEqual(_encodedPledgeId, result.EncodedPledgeId);
            result.Applications.ToList().ForEach(application =>
            {
                Assert.AreEqual("123", application.EncodedApplicationId);
                Assert.AreEqual("Awaiting approval", application.Status);
            });
            
        }

        [Test]
        public async Task GetApplicationForAsync_Returns_ValidViewModel()
        {
            var response = _fixture.Create<GetApplicationResponse>();
            _pledgeService.Setup(o => o.GetApplication(0, 0, 0, CancellationToken.None)).ReturnsAsync(response);
            
            var result = await _orchestrator.GetApplicationViewModel(new ApplicationRequest() { AccountId = 0, PledgeId = 0, ApplicationId = 0});

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.JobRole));
        }
    }
}
