using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddEncodingServiceExtensions
    {
        public static void AddEncodingService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EncodingConfig>(configuration.GetSection("EncodingService"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
            services.AddSingleton<IEncodingService, EncodingService>();
        }
    }
}
