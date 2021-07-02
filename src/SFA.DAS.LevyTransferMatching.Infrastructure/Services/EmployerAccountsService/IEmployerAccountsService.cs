using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService
{
    public interface IEmployerAccountsService
    {
        Task<bool> IsUserInRole(Guid userRef, long accountId, HashSet<UserRole> roles, CancellationToken cancellationToken = default);
        Task<IEnumerable<long>> GetUserAccounts(Guid userRef, HashSet<UserRole> roles, CancellationToken cancellationToken = default);
        Task HealthCheck(CancellationToken cancellationToken = default);
    }
}