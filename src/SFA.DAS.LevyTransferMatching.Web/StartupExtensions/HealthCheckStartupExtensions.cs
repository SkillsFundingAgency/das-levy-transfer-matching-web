﻿using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.HealthChecks;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class HealthCheckStartupExtensions
{
    public static IServiceCollection AddDasHealthChecks(this IServiceCollection services, LevyTransferMatchingWeb config)
    {
        services
            .AddHealthChecks()
            .AddCheck<ApiHealthCheck>("Api health check")
            .AddRedis(config.RedisConnectionString, "Redis health check");

        return services;
    }

    public static IApplicationBuilder UseDasHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/ping", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = (context, _) =>
            {
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("");
            }
        });

        return app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = (httpContext, healthReport) => httpContext.Response.WriteJsonAsync(new
            {
                healthReport.Status,
                healthReport.TotalDuration,
                Results = healthReport.Entries.ToDictionary(
                    key => key.Key,
                    entry => new
                    {
                        entry.Value.Status,
                        entry.Value.Duration,
                        entry.Value.Description,
                        entry.Value.Data
                    })
            })
        });
    }
}