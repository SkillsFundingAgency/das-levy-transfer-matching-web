using Microsoft.Extensions.Configuration;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddEmployerSharedUiExtensions
{
    public static void AddEmployerSharedUi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMaMenuConfiguration("signout", configuration["APPSETTING_ResourceEnvironmentName"]);

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