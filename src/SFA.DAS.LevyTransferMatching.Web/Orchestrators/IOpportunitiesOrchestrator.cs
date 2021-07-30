using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<DetailViewModel> GetDetailViewModel(int pledgeId);
        Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request);
        Task UpdateCacheItem(MoreDetailsPostRequest request);
        Task UpdateCacheItem(ApplicationDetailsPostRequest request);
        Task UpdateCacheItem(SectorPostRequest request);
        Task<string> GetUserEncodedAccountId();
        Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request);
        Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request);
        Task<ApplicationRequest> PostApplicationViewModel(ApplicationDetailsPostRequest request);
        Task<SectorViewModel> GetSectorViewModel(SectorRequest request);
        Task<GetFundingEstimateViewModel> GetFundingEstimate(GetFundingEstimateRequest request, ApplicationDetailsDto applicationDetails = null);
        Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest);
        Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest);
        Task<ConfirmationViewModel> GetConfirmationViewModel(ConfirmationRequest request);
        Task SubmitApplication(ApplyPostRequest request);

    }
}
