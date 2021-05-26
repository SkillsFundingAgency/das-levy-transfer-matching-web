using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
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
                EncodedAccountId = encodedAccountId
            };
        }

        public CreateViewModel GetCreateViewModel(string encodedAccountId)
        {
            return new CreateViewModel
            {
                EncodedAccountId = encodedAccountId
            };
        }

        public AmountViewModel GetAmountViewModel(string encodedAccountId)
        {
            return new AmountViewModel
            {
                EncodedAccountId = encodedAccountId
            };
        }
    }
}
