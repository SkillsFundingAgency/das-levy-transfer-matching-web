using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IPledgeOrchestrator
    {
        IndexViewModel GetIndexViewModel(string encodedAccountId);
        CreateViewModel GetCreateViewModel(string encodedAccountId);
        AmountViewModel GetAmountViewModel(string encodedAccountId);
    }
}