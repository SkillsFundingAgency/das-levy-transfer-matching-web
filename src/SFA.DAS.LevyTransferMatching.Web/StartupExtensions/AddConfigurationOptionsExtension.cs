using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<LevyTransferMatchingWeb>(configuration.GetSection("LevyTransferMatchingWeb"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingWeb>>().Value);

            services.Configure<LevyTransferMatchingApi>(configuration.GetSection("LevyTransferMatchingApi"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingApi>>().Value);
            
            services.Configure<Infrastructure.Configuration.Authentication>(configuration.GetSection("Authentication"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<Infrastructure.Configuration.Authentication>>().Value);
            
            services.Configure<EncodingConfig>(configuration.GetSection("EncodingService"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
            
            services.Configure<EmployerAccountsApiClientConfiguration>(configuration.GetSection("EmployerAccountApi"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsApiClientConfiguration>>().Value);
        }
    }
}
