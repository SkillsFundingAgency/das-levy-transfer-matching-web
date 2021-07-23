using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<DetailViewModel> GetDetailViewModel(int pledgeId);
        Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request);
        Task UpdateCacheItem(MoreDetailsPostRequest request);
        Task<string> GetUserEncodedAccountId();
        Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request);
        Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest);
        Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest);
    }
}
