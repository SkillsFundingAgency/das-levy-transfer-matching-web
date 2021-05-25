using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.EmployerUserRoles.Handlers;
using SFA.DAS.Authorization.Logging;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthorizationStartupExtensions
    {
        public static IServiceCollection AddDasAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddTransient<DAS.Authorization.Handlers.IAuthorizationHandler, AuthorizationResultLogger>(s => new AuthorizationResultLogger(new AuthorizationHandler(s.GetRequiredService<IEmployerAccountsApiClient>()), s.GetRequiredService<ILogger<AuthorizationResultLogger>>()));
            return services;
        }
    }
}
