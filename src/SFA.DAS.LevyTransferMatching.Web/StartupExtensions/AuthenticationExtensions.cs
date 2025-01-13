using Microsoft.Extensions.Configuration;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AuthenticationExtensions
{
    public static void AddEmployerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GovUkOidcConfiguration>(configuration.GetSection("GovUkOidcConfiguration"));
        services.AddAndConfigureGovUkAuthentication(configuration, new AuthRedirects
        {
            SignedOutRedirectUrl = string.Empty,
            LocalStubLoginPath = "/SignIn-Stub"
        }, null, typeof(AccountUserService));
    }
}