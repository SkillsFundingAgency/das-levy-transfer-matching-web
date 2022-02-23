using System;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators.Pledge
{
    public abstract class PledgeBaseOrchestrator
    {
        protected readonly ICacheStorageService _cacheStorageService;
        protected readonly IPledgeService PledgeService;

        protected PledgeBaseOrchestrator(ICacheStorageService cacheStorageService, IPledgeService pledgeService)
        {
            _cacheStorageService = cacheStorageService;
            PledgeService = pledgeService;
        }

        protected async Task<CreatePledgeCacheItem> RetrievePledgeCacheItem(Guid key)
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
