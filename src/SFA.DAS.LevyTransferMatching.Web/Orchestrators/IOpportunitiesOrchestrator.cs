using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<IndexViewModel> GetIndexViewModel(IndexRequest request);
        Task<DetailViewModel> GetDetailViewModel(int pledgeId);
        Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request);
        Task UpdateCacheItem(MoreDetailsPostRequest request);
        Task UpdateCacheItem(ApplicationDetailsPostRequest request, int amount);
        Task UpdateCacheItem(SectorPostRequest request);
        Task<SelectAccountViewModel> GetSelectAccountViewModel(SelectAccountRequest request);
        Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request);
        Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request);
        Task<ApplicationRequest> PostApplicationViewModel(ApplicationDetailsPostRequest request);
        Task<SectorViewModel> GetSectorViewModel(SectorRequest request);
        Task<GetFundingEstimateViewModel> GetFundingEstimate(GetFundingEstimateRequest request, GetApplicationDetailsResponse applicationDetails = null);
        Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest);
        Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest);
        Task<ConfirmationViewModel> GetConfirmationViewModel(ConfirmationRequest request);
        Task SubmitApplication(ApplyPostRequest request);

    }
}
