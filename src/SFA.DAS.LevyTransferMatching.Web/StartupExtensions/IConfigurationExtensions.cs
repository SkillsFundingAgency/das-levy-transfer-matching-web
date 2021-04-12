using Microsoft.Extensions.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class ConfigurationExtensions
    {
        public static T GetSection<T>(this IConfiguration configuration)
        {
            return configuration
                .GetSection(typeof(T).Name)
                .Get<T>();
        }
    }
}
