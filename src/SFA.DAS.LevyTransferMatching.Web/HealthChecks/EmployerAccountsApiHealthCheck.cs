using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService;

namespace SFA.DAS.LevyTransferMatching.Web.HealthChecks
{
    public class EmployerAccountsApiHealthCheck : IHealthCheck
    {
        private readonly IEmployerAccountsService _service;

        public EmployerAccountsApiHealthCheck(IEmployerAccountsService service)
        {
            _service = service;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var description = "Employer Accounts Cosmos Db Health probe [used in Employer Authorization]";

            try
            {
                await _service.HealthCheck(cancellationToken);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description, ex);
            }

            return HealthCheckResult.Healthy(description, new Dictionary<string, object>());
        }
    }
}
