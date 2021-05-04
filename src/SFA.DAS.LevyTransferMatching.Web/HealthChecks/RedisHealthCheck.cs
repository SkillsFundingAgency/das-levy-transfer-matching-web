using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.LevyTransferMatching.Web.HealthChecks
{
    //public class RedisHealthCheck
    //{
    //    private readonly string _redisConnectionString;

    //    public RedisHealthCheck(LevyTransferMatchingWeb config)
    //    {
    //        _redisConnectionString = config.RedisConnectionString;
    //    }

    //    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
    //        CancellationToken cancellationToken = new CancellationToken())
    //    {
    //        try
    //        {
    //            var conn = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
    //            var sub = conn.GetSubscriber();
    //            var timeSpan = await sub.PingAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            return HealthCheckResult.Unhealthy("Redis ping", ex);
    //        }

    //        return HealthCheckResult.Healthy("Redis ping", new Dictionary<string, object>());
    //    }
    //}
}
