using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class CookieTempDataProviderOptionsExtensions
    {
        public static IServiceCollection AddCookieTempDataProvider(this IServiceCollection services)
        {
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            return services;
        }
    }
}
