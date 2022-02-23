using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators.Pledge
{
    public interface IAmountOrchestrator
    {
        public Task<AmountViewModel> GetAmountViewModel(AmountRequest request);
        Task UpdateCacheItem(AmountPostRequest request);
    }

    public class AmountOrchestrator : PledgeBaseOrchestrator, IAmountOrchestrator
    {
        public AmountOrchestrator(ICacheStorageService cacheStorageService, IPledgeService pledgeService)
            : base(cacheStorageService, pledgeService)
        {

        }

        public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var accountDataTask = PledgeService.GetAmount(request.EncodedAccountId);

            await Task.WhenAll(cacheItemTask, accountDataTask);
            var cacheItem = cacheItemTask.Result;
            var accountData = accountDataTask.Result;

            return new AmountViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount.ToString(),
                RemainingTransferAllowance = accountData.RemainingTransferAllowance.ToString("N0"),
                IsNamePublic = cacheItem.IsNamePublic,
                DasAccountName = accountData.DasAccountName
            };
        }

        public async Task UpdateCacheItem(AmountPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Amount = int.Parse(request.Amount, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            cacheItem.IsNamePublic = request.IsNamePublic.Value;
            cacheItem.DasAccountName = request.DasAccountName;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }
    }
}
