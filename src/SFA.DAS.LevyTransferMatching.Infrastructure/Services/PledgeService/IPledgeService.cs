using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task<GetPledgesResponse> GetPledges(long accountId, int? page = 1, int? pageSize = null);
        Task<long> PostPledge(CreatePledgeRequest request, long accountId);
        Task RejectApplications(SetRejectApplicationsRequest request, long accountId, int pledgeId);
        Task<GetCreateResponse> GetCreate(long accountId);
        Task<GetAmountResponse> GetAmount(string encodedAccountId);
        Task<GetOrganisationNameResponse> GetOrganisationName(string encodedAccountId);
        Task<GetSectorResponse> GetSector(long accountId);
        Task<GetJobRoleResponse> GetJobRole(long accountId);
        Task<GetLevelResponse> GetLevel(long accountId);
        Task<GetApplicationsResponse> GetApplications(long accountId, int pledgeId, SortColumn? sortOrder, SortOrder? sortDirection, int? page = 1, int? pageSize = null);
		Task<GetApplicationResponse> GetApplication(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationOutcome(long accountId, int applicationId, int pledgeId, SetApplicationOutcomeRequest outcomeRequest);
        Task<GetApplicationApprovedResponse> GetApplicationApproved(long accountId, int pledgeId, int applicationId);
        Task<GetApplicationApprovalOptionsResponse> GetApplicationApprovalOptions(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationApprovalOptions(long accountId, int applicationId, int pledgeId, SetApplicationApprovalOptionsRequest request);
        Task ClosePledge(long accountId, int pledgeId, ClosePledgeRequest request);
        Task<GetRejectApplicationsResponse> GetRejectApplications(long accountId, int pledgeId);
    }
}