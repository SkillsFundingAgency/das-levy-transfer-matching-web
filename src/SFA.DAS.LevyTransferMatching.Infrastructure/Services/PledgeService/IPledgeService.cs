using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task<GetPledgesResponse> GetPledges(long accountId);
        Task<long> PostPledge(CreatePledgeRequest request, long accountId);
        Task RejectApplications(SetRejectApplicationsRequest request, long accountId, int pledgeId);
        Task<GetCreateResponse> GetCreate(long accountId);
        Task<GetAmountResponse> GetAmount(string encodedAccountId);
        Task<GetSectorResponse> GetSector(long accountId);
        Task<GetJobRoleResponse> GetJobRole(long accountId);
        Task<GetLevelResponse> GetLevel(long accountId);
        Task<GetApplicationsResponse> GetApplications(long accountId, int pledgeId);
		Task<GetApplicationResponse> GetApplication(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationOutcome(long accountId, int applicationId, int pledgeId, SetApplicationOutcomeRequest outcomeRequest);
        Task<GetApplicationApprovedResponse> GetApplicationApproved(long accountId, int pledgeId, int applicationId);
        Task<GetApplicationApprovalOptionsResponse> GetApplicationApprovalOptions(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationApprovalOptions(long accountId, int applicationId, int pledgeId, SetApplicationApprovalOptionsRequest request);
        Task ClosePledge(long accountId, int pledgeId, ClosePledgeRequest request);
        Task<GetApplicationsAccountNamesResponse> GetApplicationsDasNames(long accountId, int pledgeId);
    }
}