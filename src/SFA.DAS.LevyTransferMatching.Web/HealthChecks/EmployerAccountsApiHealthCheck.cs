using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.LevyTransferMatching.Web.HealthChecks
{
    public class EmployerAccountsApiHealthCheck : IHealthCheck
    {
        private readonly IEmployerAccountsApiClient _apiClient;

        public EmployerAccountsApiHealthCheck(IEmployerAccountsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var description = "Employer Accounts Cosmos Db Health probe [used in Employer Authorization]";

            try
            {
                await _apiClient.Ping(cancellationToken);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description, ex);
            }

            return HealthCheckResult.Healthy(description, new Dictionary<string, object>());
        }
    }
}
