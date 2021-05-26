using System;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService)
        {
            _cacheStorageService = cacheStorageService;
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
                IsNamePublic = cacheItem.IsNamePublic
            };
        }

        public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);
        
            return new AmountViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount.ToString(),
                IsNamePublic = cacheItem.IsNamePublic
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            return new SectorViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey
            };
        }

        public async Task UpdateCacheItem(AmountPostRequest amountPostRequest)
        {
            var cacheItem = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(amountPostRequest.CacheKey.ToString());

            cacheItem.Amount = Int32.Parse(amountPostRequest.Amount);
            cacheItem.IsNamePublic = amountPostRequest.IsNamePublic.Value;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public Task UpdateCacheItem(SectorPostRequest request)
        {
            throw new NotImplementedException();
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
