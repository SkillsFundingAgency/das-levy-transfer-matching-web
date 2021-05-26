using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class CacheStartupExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IHostEnvironment environment, LevyTransferMatchingWeb config)
        {
            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(
                    options => { options.Configuration = config.RedisConnectionString; });
            }

            services.AddTransient<ICacheStorageService, CacheStorageService>();

            return services;
        }
    }
}
