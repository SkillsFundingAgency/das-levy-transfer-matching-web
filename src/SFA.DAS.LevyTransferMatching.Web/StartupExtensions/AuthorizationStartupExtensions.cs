using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.EmployerUserRoles.Handlers;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthorizationStartupExtensions
    {
        public static IServiceCollection AddDasAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddTransient<DAS.Authorization.Handlers.IAuthorizationHandler, AuthorizationHandler>();
            return services;
        }
    }
}
