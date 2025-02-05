using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class ConfigurationKeys
{
    public const string Authentication = nameof(Authentication);
    public const string CosmosDb = nameof(CosmosDb);
    public const string EncodingConfig = "SFA.DAS.Encoding";
    public const string FeatureToggles = nameof(FeatureToggles);
    public const string LevyTransferMatchingWeb = nameof(LevyTransferMatchingWeb);
    public const string LevyTransferMatchingApi = nameof(LevyTransferMatchingApi);
}

public static class AddConfigurationOptionsExtension
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        
        services.Configure<LevyTransferMatchingWeb>(configuration.GetSection(ConfigurationKeys.LevyTransferMatchingWeb));
        services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingWeb>>().Value);

        services.Configure<LevyTransferMatchingApi>(configuration.GetSection(ConfigurationKeys.LevyTransferMatchingApi));
        services.AddSingleton(cfg => cfg.GetService<IOptions<LevyTransferMatchingApi>>().Value);

        services.Configure<Infrastructure.Configuration.Authentication>(configuration.GetSection(ConfigurationKeys.Authentication));
        services.AddSingleton(cfg => cfg.GetService<IOptions<Infrastructure.Configuration.Authentication>>().Value);

        var encodingConfigJson = configuration.GetSection(ConfigurationKeys.EncodingConfig).Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        services.Configure<CosmosDbConfiguration>(configuration.GetSection(ConfigurationKeys.CosmosDb));
        services.AddSingleton(cfg => cfg.GetService<IOptions<CosmosDbConfiguration>>().Value);

        services.Configure<Infrastructure.Configuration.FeatureToggles>(configuration.GetSection(ConfigurationKeys.FeatureToggles));
        services.AddSingleton(cfg => cfg.GetService<IOptions<Infrastructure.Configuration.FeatureToggles>>().Value);
    }
}