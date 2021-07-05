using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Api
{
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly IAccountUsersReadOnlyRepository _accountUsersRepository;

        public EmployerAccountsApiClient(IAccountUsersReadOnlyRepository repository)
        {
            _accountUsersRepository = repository;
        }

        public async Task<bool> IsUserInRole(IsUserInRoleRequest request, CancellationToken cancellationToken)
        {
            return await _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r =>
                    r.userRef == request.UserRef &&
                    r.accountId == request.AccountId &&
                    r.removed == null &&
                    r.role != null && request.Roles.Contains(r.role.Value), cancellationToken);
        }

        public async Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest request, CancellationToken cancellationToken)
        {
            return await _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r =>
                    r.userRef == request.UserRef &&
                    r.accountId == request.AccountId &&
                    r.removed == null, cancellationToken);
        }

        public async Task Ping(CancellationToken cancellationToken)
        {
            var options = new FeedOptions { EnableCrossPartitionQuery = true };
            var value = await _accountUsersRepository
                .CreateQuery(options)
                .AnyAsync(z => z.accountId > 0, cancellationToken);

            if (!value)
            {
                throw new InvalidOperationException("EmployerAccountsApiClient health check failed - db is empty");
            }
        }
    }
}
