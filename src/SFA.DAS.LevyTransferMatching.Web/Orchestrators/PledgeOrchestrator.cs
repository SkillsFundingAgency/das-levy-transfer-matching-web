using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Enums;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IAccountsService _accountsService;
        private readonly IPledgesService _pledgesService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService, IAccountsService accountsService, IPledgesService pledgesService)
        {
            _cacheStorageService = cacheStorageService;
            _accountsService = accountsService;
            _pledgesService = pledgesService;
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

            return new CreateViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount,
                IsNamePublic = cacheItem.IsNamePublic,
                Sectors = cacheItem.Sectors,
                JobRoles = cacheItem.JobRoles,
                Levels = cacheItem.Levels
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

            return new SectorViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Sectors = cacheItem.Sectors
            };
        }

        public async Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            return new JobRoleViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                JobRoles = cacheItem.JobRoles
            };
        }

        public async Task SubmitPledge(CreateRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            ValidateCacheItem(cacheItem);

            var pledgeDto = new PledgeDto
            {
                Amount = (int)cacheItem.Amount,
                IsNamePublic = (bool)cacheItem.IsNamePublic,
                Sectors = GetFlagsAsList(cacheItem.Sectors.Value),
                JobRoles = GetFlagsAsList(cacheItem.JobRoles.Value),
                Levels = GetFlagsAsList(cacheItem.Levels.Value)
            };

            await _pledgesService.PostPledge(pledgeDto, request.EncodedAccountId);
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

            return new LevelViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Levels = cacheItem.Levels,
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

        private List<T> GetFlagsAsList<T>(T sectors)
        {
            return sectors.ToString().Split(", ").Select(x => (T)Enum.Parse(typeof(T), x)).ToList();
        }

        private void ValidateCacheItem(CreatePledgeCacheItem createPledgeCacheItem)
        {
            if (!createPledgeCacheItem.Amount.HasValue || !createPledgeCacheItem.IsNamePublic.HasValue)
                throw new Exception("Cache item must have value");

            if (!createPledgeCacheItem.Sectors.HasValue)
                createPledgeCacheItem.Sectors = Sector.None;
            if (!createPledgeCacheItem.JobRoles.HasValue)
                createPledgeCacheItem.JobRoles = JobRole.None;
            if (!createPledgeCacheItem.Levels.HasValue)
                createPledgeCacheItem.Levels = Level.None;
        }
    }
}
