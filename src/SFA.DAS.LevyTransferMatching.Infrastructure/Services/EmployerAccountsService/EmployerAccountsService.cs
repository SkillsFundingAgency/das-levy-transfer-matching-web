using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService
{
    public class EmployerAccountsService : IEmployerAccountsService
    {
        private readonly IAccountUsersReadOnlyRepository _accountUsersRepository;

        public EmployerAccountsService(IAccountUsersReadOnlyRepository accountUsersRepository)
        {
            _accountUsersRepository = accountUsersRepository;
        }

        public async Task<bool> IsUserInRole(Guid userRef, long accountId, HashSet<UserRole> roles, CancellationToken cancellationToken)
        {
            return await _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r =>
                    r.userRef == userRef &&
                    r.accountId == accountId &&
                    r.removed == null &&
                    r.role != null && roles.Contains(r.role.Value), cancellationToken);
        }

        public async Task<bool> IsUserInAnyRole(Guid userRef, long accountId, CancellationToken cancellationToken)
        {
            return await _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r =>
                    r.userRef == userRef &&
                    r.accountId == accountId &&
                    r.removed == null, cancellationToken);
        }

        public async Task<IEnumerable<long>> GetUserAccounts(Guid userRef, HashSet<UserRole> roles, CancellationToken cancellationToken = default)
        {
            var options = new FeedOptions { EnableCrossPartitionQuery = true };
            var accountUsers = await _accountUsersRepository
                .CreateQuery(options)
                .Where(r =>
                    r.userRef == userRef &&
                    r.removed == null &&
                    r.role != null &&
                    roles.Contains(r.role.Value))
                .ToListAsync(cancellationToken: cancellationToken);

            return accountUsers.Select(x => x.accountId);
        }

        public async Task HealthCheck(CancellationToken cancellationToken)
        {
            var options = new FeedOptions { EnableCrossPartitionQuery = true };
            var value = await _accountUsersRepository
                .CreateQuery(options)
                .AnyAsync(z => z.accountId > 0, cancellationToken);

            if (!value)
            {
                throw new InvalidOperationException("EmployerAccountsService health check failed - db is empty");
            }
        }
    }
}
