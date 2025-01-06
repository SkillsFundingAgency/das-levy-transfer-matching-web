using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddConfigurationOptionsExtension
{
    private static readonly Dictionary<Type, string> ConfigSections = new()
    {
        { typeof(LevyTransferMatchingWeb), nameof(LevyTransferMatchingWeb) },
        { typeof(LevyTransferMatchingApi), nameof(LevyTransferMatchingApi) },
        { typeof(Infrastructure.Configuration.Authentication), "Authentication" },
        { typeof(EncodingConfig), "EncodingService" },
        { typeof(CosmosDbConfiguration), "CosmosDb" },
        { typeof(Infrastructure.Configuration.FeatureToggles), "FeatureToggles" },
    };

    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        
        foreach (var configSection in ConfigSections)
        {
            services.AddOptionsFor(configuration, configSection.Key,  configSection.Value);
        }
    }

    private static void AddOptionsFor<T>(this IServiceCollection services, IConfiguration configuration, T type, string sectionName) where T : class
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        services.AddSingleton(cfg => cfg.GetService<IOptions<T>>().Value);
    }
}