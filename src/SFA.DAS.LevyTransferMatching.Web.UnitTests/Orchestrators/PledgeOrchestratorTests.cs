using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class PledgeOrchestratorTests
    {
        private PledgeOrchestrator _orchestrator;
        private Fixture _fixture;
        private Mock<ICacheStorageService> _cache;
        private Mock<IAccountsService> _accountsService;
        private Mock<IPledgesService> _pledgesService;
        private Mock<ITagService> _tagService;
        private List<Tag> _sectors;
        private List<Tag> _levels;
        private List<Tag> _jobRoles;
        private string _encodedAccountId;
        private Guid _cacheKey;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _encodedAccountId = _fixture.Create<string>();
            _cacheKey = Guid.NewGuid();
            _cache = new Mock<ICacheStorageService>();
            _accountsService = new Mock<IAccountsService>();
            _pledgesService = new Mock<IPledgesService>();

            _sectors = _fixture.Create<List<Tag>>();
            _levels = _fixture.Create<List<Tag>>();
            _jobRoles = _fixture.Create<List<Tag>>();
            _tagService = new Mock<ITagService>();
            _tagService.Setup(x => x.GetJobRoles()).ReturnsAsync(_jobRoles);
            _tagService.Setup(x => x.GetSectors()).ReturnsAsync(_sectors);
            _tagService.Setup(x => x.GetLevels()).ReturnsAsync(_levels);

            _orchestrator = new PledgeOrchestrator(_cache.Object, _accountsService.Object, _pledgesService.Object, _tagService.Object);
        }

        [Test]
        public void GetIndexViewModel_EncodedId_Is_Correct()
        {
            var result = _orchestrator.GetIndexViewModel(_encodedAccountId);
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public void GetIndexViewModel_CacheKey_Has_Value()
        {
            var result = _orchestrator.GetIndexViewModel(_encodedAccountId);
            Assert.AreNotEqual(Guid.Empty, result.CacheKey);
        }

        [Test]
        public async Task GetCreateViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest{ EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey});
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetCreateViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetCreateViewModel_Amount_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Amount, result.Amount);
        }

        [Test]
        public async Task GetCreateViewModel_SectorOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_sectors, result.SectorOptions);
        }

        [Test]
        public async Task GetCreateViewModel_JobRoleOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_jobRoles, result.JobRoleOptions);
        }

        [Test]
        public async Task GetCreateViewModel_LevelOptions_Are_Populated()
        {
            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_levels, result.LevelOptions);
        }

        [Test]
        public async Task GetCreateViewModel_IsNamePublic_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Create<CreatePledgeCacheItem>();
            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.IsNamePublic, result.IsNamePublic);
        }


        [Test]
        public async Task GetCreateViewModel_Sectors_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                .With(x => x.Sectors, new List<string>{"Business"})
                .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Sectors, result.Sectors);
        }

        [Test]
        public async Task GetCreateViewModel_JobRoles_Is_Retrieved_From_Cache()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                .With(x => x.JobRoles, new List<string>{ "Business"})
                .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetCreateViewModel(new CreateRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
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
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetSectorViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetSectorViewModel_SectorOptions_Are_Populated()
        {
            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_sectors, result.SectorOptions);
        }

        [Test]
        public async Task GetSectorViewModel_Sectors_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.Sectors, new List<string> { "Agriculture" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetSectorViewModel(new SectorRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Sectors, result.Sectors);
        }

        [Test]
        public async Task GetJobRoleViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetJobRoleViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetJobRoleViewModel_JobRoleOptions_Are_Populated()
        {
            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_jobRoles, result.JobRoleOptions);
        }

        [Test]
        public async Task GetJobRoleViewModel_JobRoles_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.JobRoles, new List<string> { "Business" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetJobRoleViewModel(new JobRoleRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.JobRoles, result.JobRoles);
        }

        [Test]
        public async Task GetLevelViewModel_EncodedId_Is_Correct()
        {
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_encodedAccountId, result.EncodedAccountId);
        }

        [Test]
        public async Task GetLevelViewModel_CacheKey_Is_Correct()
        {
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_cacheKey, result.CacheKey);
        }

        [Test]
        public async Task GetLevelViewModel_LevelOptions_Are_Populated()
        { 
            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(_levels, result.LevelOptions);
        }

        [Test]
        public async Task GetLevelViewModel_Sectors_Is_Correct()
        {
            var cacheItem = _fixture.Build<CreatePledgeCacheItem>()
                    .With(x => x.Levels, new List<string> { "Level2" })
                    .Create();

            _cache.Setup(x => x.RetrieveFromCache<CreatePledgeCacheItem>(_cacheKey.ToString())).ReturnsAsync(cacheItem);

            var result = await _orchestrator.GetLevelViewModel(new LevelRequest { EncodedAccountId = _encodedAccountId, CacheKey = _cacheKey });
            Assert.AreEqual(cacheItem.Levels, result.Levels);
        }
    }
}
