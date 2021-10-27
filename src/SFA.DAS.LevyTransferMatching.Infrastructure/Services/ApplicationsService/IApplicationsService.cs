using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService
{
    public interface IApplicationsService
    {
        Task<GetApplicationsResponse> GetApplications(long accountId, CancellationToken cancellationToken = default);
        Task<Types.GetApplicationResponse> GetApplication(long accountId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationAcceptance(SetApplicationAcceptanceRequest request, CancellationToken cancellationToken = default);
        Task<GetAcceptedResponse> GetAccepted(long accountId, int applicationId);
        Task<GetDeclinedResponse> GetDeclined(long accountId, int applicationId);
    }
}