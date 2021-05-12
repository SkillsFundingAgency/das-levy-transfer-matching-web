using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Stubs
{
    public class StubEmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        public StubEmployerAccountsApiClient()
        {
        }

        public Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task Ping(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }
    }
}
