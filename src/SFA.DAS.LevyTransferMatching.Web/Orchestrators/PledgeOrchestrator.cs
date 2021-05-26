using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService)
        {
            _cacheStorageService = cacheStorageService;
        }
    }
}
