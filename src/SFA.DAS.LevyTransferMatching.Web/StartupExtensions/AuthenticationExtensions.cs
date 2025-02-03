using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AuthenticationExtensions
{
    public static void AddEmployerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GovUkOidcConfiguration>(configuration.GetSection("GovUkOidcConfiguration"));
        services.AddAndConfigureGovUkAuthentication(configuration,typeof(PostAuthenticationClaimsHandler), "", "/SignIn-Stub");
    }
}