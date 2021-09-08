﻿using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task<GetPledgesResponse> GetPledges(long accountId);
        Task<long> PostPledge(CreatePledgeRequest request, long accountId);
        Task<GetCreateResponse> GetCreate(long accountId);
        Task<GetAmountResponse> GetAmount(string encodedAccountId);
        Task<GetSectorResponse> GetSector(long accountId);
        Task<GetJobRoleResponse> GetJobRole(long accountId);
        Task<GetLevelResponse> GetLevel(long accountId);
        Task<GetApplicationsResponse> GetApplications(long accountId, int pledgeId);
		Task<GetApplicationResponse> GetApplication(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default);
        Task<GetApplicationApprovedResponse> GetApplicationApproved(long accountId, int pledgeId, int applicationId);
    }
}