﻿using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService
{
    public interface IApplicationsService
    {
        Task<GetApplicationsResponse> GetApplications(long accountId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<GetApplicationResponse> GetApplication(long accountId, int applicationId, CancellationToken cancellationToken = default);
        Task SetApplicationAcceptance(SetApplicationAcceptanceRequest request, CancellationToken cancellationToken = default);
        Task<GetAcceptedResponse> GetAccepted(long accountId, int applicationId);
        Task<GetDeclinedResponse> GetDeclined(long accountId, int applicationId);
        Task<GetWithdrawnResponse> GetWithdrawn(long accountId, int applicationId);
        Task<GetWithdrawalConfirmationResponse> GetWithdrawalConfirmation(long accountId, int applicationId);
        Task WithdrawApplicationAfterAcceptance(WithdrawApplicationAfterAcceptanceRequest request, long accountId, int applicationId);
    }
}