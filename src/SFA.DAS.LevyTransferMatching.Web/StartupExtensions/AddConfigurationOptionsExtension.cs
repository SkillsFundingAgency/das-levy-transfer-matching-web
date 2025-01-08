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
        
        services.AddOptionsFor<LevyTransferMatchingWeb>(configuration, nameof(LevyTransferMatchingWeb));
        services.AddOptionsFor<LevyTransferMatchingApi>(configuration, nameof(LevyTransferMatchingApi));
        services.AddOptionsFor<Infrastructure.Configuration.Authentication>(configuration, "Authentication");
        services.AddOptionsFor<EncodingConfig>(configuration, "EncodingService");
        services.AddOptionsFor<CosmosDbConfiguration>(configuration, "CosmosDb");
        services.AddOptionsFor<Infrastructure.Configuration.FeatureToggles>(configuration, "FeatureToggles");
 }

    private static void AddOptionsFor<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        services.AddSingleton(cfg => cfg.GetService<IOptions<T>>().Value);
    }
}