using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class DataProtectionStartupExtensions
{
    public static IServiceCollection AddDasDataProtection(this IServiceCollection services, LevyTransferMatchingWeb webConfiguration, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return services;
        }
        
        var redisConnectionString = webConfiguration.RedisConnectionString;
        var dataProtectionKeysDatabase = webConfiguration.DataProtectionKeysDatabase;

        var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName("das-employer")
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        
        return services;
    }
}