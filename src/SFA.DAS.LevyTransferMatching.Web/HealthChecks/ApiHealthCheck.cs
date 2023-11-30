using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;

namespace SFA.DAS.LevyTransferMatching.Web.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly IApiClient _apiClient;

        public ApiHealthCheck(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var description = "Ping of Levy Transfer Matching APIM inner API";

            try
            {
                await _apiClient.Ping();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description, ex);
            }

            return HealthCheckResult.Healthy(description, new Dictionary<string, object>());
        }
    }
}
