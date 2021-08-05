using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService
{
    public interface IOpportunitiesService
    {
        Task<List<OpportunityDto>> GetAllOpportunities();
        Task<GetApplyResponse> GetApply(long accountId, int opportunityId);
        Task<GetContactDetailsResponse> GetContactDetails(long accountId, int pledgeId); 
        Task<GetApplicationDetailsResponse> GetApplicationDetails(long accountId, int id, string standardId = default);
        Task<GetMoreDetailsResponse> GetMoreDetails(long accountId, int pledgeId);
        Task<GetSectorResponse> GetSector(long accountId, int pledgeId);
        Task<GetSectorResponse> GetSector(long accountId, int pledgeId, string postcode);
		Task<GetConfirmationResponse> GetConfirmation(long accountId, int opportunityId);
        Task<ApplyResponse> PostApplication(long accountId, int opportunityId, ApplyRequest request);
        Task<GetDetailResponse> GetDetail(int pledgeId);
    }
}