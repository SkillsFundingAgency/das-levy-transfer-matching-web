﻿using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
#endif
            .AddEnvironmentVariables();

        if (!configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["Environment"];
                    options.PreFixConfigurationKeys = false;
                    options.ConfigurationKeysRawJsonResult = ["SFA.DAS.Encoding"];
                }
            );
        }

        return config.Build();
    }
        
    public static T GetSection<T>(this IConfiguration configuration)
    {
        return configuration
            .GetSection(typeof(T).Name)
            .Get<T>();
    }
}