using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.LevyTransferMatching.Domain.EmployerAccounts;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Api
{
    public class EmployerAccountsApiClient : ApiClientBase, IEmployerAccountsApiClient
    {
        private readonly EmployerAccountsApiClientConfiguration _configuration;

        public EmployerAccountsApiClient(EmployerAccountsApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> IsUserInRole(IsUserInRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            var user = await GetUser(roleRequest.AccountId, roleRequest.UserRef, cancellationToken);
            if (user == null) return false;

            foreach (var role in user.Roles)
            {
                if (roleRequest.Roles.Contains(role))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> IsUserInAnyRole(IsUserInAnyRoleRequest roleRequest, CancellationToken cancellationToken)
        {
            var user = await GetUser(roleRequest.AccountId, roleRequest.UserRef, cancellationToken);
            return user != null && user.Roles.Any();
        }

        public async Task Ping(CancellationToken cancellationToken)
        {
            await Get(new PingRequest(_configuration.ApiBaseUrl));
        }

        private async Task<AccountUser> GetUser(long accountId, Guid userRef, CancellationToken cancellationToken)

        {
            var users = await GetUsers(accountId, cancellationToken);
            return users.FirstOrDefault(x => x.UserRef == userRef);
        }

        private async Task<IEnumerable<AccountUser>> GetUsers(long accountId, CancellationToken cancellationToken)
        {
            var request = new GetAccountUsersApiRequest(_configuration.ApiBaseUrl, accountId);
            return await GetAll<AccountUser>(request);
        }

        protected override async Task<string> GetAccessTokenAsync()
        {
            var clientCredential = new ClientCredential(_configuration.ClientId, _configuration.ClientSecret);
            var context = new AuthenticationContext($"https://login.microsoftonline.com/{_configuration.Tenant}", true);

            var result = await context.AcquireTokenAsync(_configuration.IdentifierUri, clientCredential).ConfigureAwait(false);

            return result.AccessToken;
        }
    }
}
