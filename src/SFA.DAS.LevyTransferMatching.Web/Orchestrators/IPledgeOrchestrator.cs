using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IPledgeOrchestrator
    {
        IndexViewModel GetIndexViewModel(string encodedAccountId);
        Task<CreateViewModel> GetCreateViewModel(CreateRequest request);
        Task<AmountViewModel> GetAmountViewModel(AmountRequest request);
        Task UpdateCacheItem(AmountPostRequest amountPostRequest);
    }
}