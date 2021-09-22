using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService
{
    public interface IApplicationsService
    {
        Task<GetApplicationsResponse> GetApplications(long accountId, CancellationToken cancellationToken = default);
        Task<GetApplicationStatusResponse> GetApplicationStatus(long accountId, int applicationId);
    }
}
