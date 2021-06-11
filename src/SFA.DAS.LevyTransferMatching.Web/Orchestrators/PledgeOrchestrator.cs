using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IAccountsService _accountsService;
        private readonly IPledgesService _pledgesService;
        private readonly ITagService _tagService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService, IAccountsService accountsService, IPledgesService pledgesService, ITagService tagService)
        {
            _cacheStorageService = cacheStorageService;
            _accountsService = accountsService;
            _pledgesService = pledgesService;
            _tagService = tagService;
        }

        public IndexViewModel GetIndexViewModel(string encodedAccountId)
        {
            return new IndexViewModel
            {
                EncodedAccountId = encodedAccountId,
                CacheKey = Guid.NewGuid()
            };
        }

        public async Task<CreateViewModel> GetCreateViewModel(CreateRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
            var levels = await _tagService.GetLevels();
            var sectors = await _tagService.GetSectors();
            var jobRoles = await _tagService.GetJobRoles();

            return new CreateViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount,
                IsNamePublic = cacheItem.IsNamePublic,
                Sectors = cacheItem.Sectors,
                JobRoles = cacheItem.JobRoles,
                Levels = cacheItem.Levels,
                LevelOptions = levels,
                SectorOptions = sectors,
                JobRoleOptions = jobRoles
            };
        }

        public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
            var remainingTransferAllowance = await _accountsService.GetRemainingTransferAllowance(request.EncodedAccountId);
        
            return new AmountViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount.ToString(),
                RemainingTransferAllowance = remainingTransferAllowance.ToString("N0"),
                IsNamePublic = cacheItem.IsNamePublic
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
            var sectors = await _tagService.GetSectors();

            return new SectorViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Sectors = cacheItem.Sectors,
                SectorOptions = sectors
            };
        }

        public async Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
            var jobRoles = await _tagService.GetJobRoles();

            return new JobRoleViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                JobRoles = cacheItem.JobRoles,
                JobRoleOptions = jobRoles
            };
        }

        public async Task SubmitPledge(CreatePostRequest request)
        {
            var cacheItem = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(request.CacheKey.ToString());

            if (cacheItem == null)
            {
                throw new InvalidOperationException("Unable to submit pledge due to cache expiry");
            }

            var pledgeDto = new PledgeDto
            {
                Amount = cacheItem.Amount.Value,
                IsNamePublic = cacheItem.IsNamePublic.Value,
                Sectors = cacheItem.Sectors ?? new List<string>(),
                JobRoles = cacheItem.JobRoles ?? new List<string>(),
                Levels = cacheItem.Levels ?? new List<string>()
            };

            await _pledgesService.PostPledge(pledgeDto, request.EncodedAccountId);
            await _cacheStorageService.DeleteFromCache(request.CacheKey.ToString());
        }

        public async Task UpdateCacheItem(AmountPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Amount = Int32.Parse(request.Amount);
            cacheItem.IsNamePublic = request.IsNamePublic.Value;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(SectorPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Sectors = request.Sectors;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(JobRolePostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.JobRoles = request.JobRoles;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(LevelPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Levels = request.Levels;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task<LevelViewModel> GetLevelViewModel(LevelRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
            var levels = await _tagService.GetLevels();

            return new LevelViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Levels = cacheItem.Levels,
                LevelOptions = levels
            };
        }

        private async Task<CreatePledgeCacheItem> RetrievePledgeCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreatePledgeCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }
    }
}
