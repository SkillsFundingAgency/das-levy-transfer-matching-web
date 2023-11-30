using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDasDataProtection(this IServiceCollection services, LevyTransferMatchingWeb webConfiguration, IHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var redisConnectionString = webConfiguration.RedisConnectionString;
                var dataProtectionKeysDatabase = webConfiguration.DataProtectionKeysDatabase;

                var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                services.AddDataProtection()
                    .SetApplicationName("das-employer")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
            return services;
        }
    }
}
