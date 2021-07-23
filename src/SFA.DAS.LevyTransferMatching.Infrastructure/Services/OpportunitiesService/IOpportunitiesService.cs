using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService
{
    public interface IOpportunitiesService
    {
        Task<List<OpportunityDto>> GetAllOpportunities();

        Task<OpportunityDto> GetOpportunity(int id);

        Task<GetConfirmationResponse> GetConfirmation(int opportunityId);
    }
}