using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddConfigurationOptionsExtension
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<LevyTransferMatchingWeb>(configuration.GetSection("LevyTransferMatchingWeb"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingWeb>>().Value);

        services.Configure<LevyTransferMatchingApi>(configuration.GetSection("LevyTransferMatchingApi"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingApi>>().Value);
            
        services.Configure<EncodingConfig>(configuration.GetSection("EncodingService"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
            
        services.Configure<CosmosDbConfiguration>(configuration.GetSection("CosmosDb"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<CosmosDbConfiguration>>().Value);

        services.Configure<Infrastructure.Configuration.FeatureToggles>(configuration.GetSection("FeatureToggles"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<Infrastructure.Configuration.FeatureToggles>>().Value);
    }
}