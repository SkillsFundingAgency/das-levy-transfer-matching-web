using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;

namespace SFA.DAS.LevyTransferMatching.Web.HealthChecks;

public class ApiHealthCheck(IApiClient apiClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        const string description = "Ping of Levy Transfer Matching APIM inner API";

        try
        {
            await apiClient.Ping();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(description, ex);
        }

        return HealthCheckResult.Healthy(description, new Dictionary<string, object>());
    }
}