using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddEmployerSharedUIExtensions
    {
        public static void AddEmployerSharedUI(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"] != null
                && configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddMaMenuConfiguration("signout", configuration["APPSETTING_ResourceEnvironmentName"]);
            }
            else
            {
                var authenticationConfig = configuration.GetSection<Infrastructure.Configuration.Authentication>();
                services.AddMaMenuConfiguration("signout", authenticationConfig.ClientId, configuration["APPSETTING_ResourceEnvironmentName"]);    
            }
            

            services.AddSingleton<ICookieBannerViewModel>(provider =>
            {
                var maLinkGenerator = provider.GetService<ILinkGenerator>();
                return new CookieBannerViewModel
                {
                    CookieDetailsUrl = maLinkGenerator.AccountsLink("cookieConsent/details"),
                    CookieConsentUrl = maLinkGenerator.AccountsLink("cookieConsent"),

                };
            });
        }
    }
}
