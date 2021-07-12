using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IPledgeOrchestrator
    {
        InformViewModel GetInformViewModel(string encodedAccountId);
        Task<CreateViewModel> GetCreateViewModel(CreateRequest request);
        Task<AmountViewModel> GetAmountViewModel(AmountRequest request);
        Task<SectorViewModel> GetSectorViewModel(SectorRequest request);
        Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request);
        Task UpdateCacheItem(AmountPostRequest request);
        Task UpdateCacheItem(SectorPostRequest request);
        Task UpdateCacheItem(JobRolePostRequest request);
        Task UpdateCacheItem(LevelPostRequest request);
        Task<LevelViewModel> GetLevelViewModel(LevelRequest request);
        Task<string> SubmitPledge(CreatePostRequest request);
    }
}