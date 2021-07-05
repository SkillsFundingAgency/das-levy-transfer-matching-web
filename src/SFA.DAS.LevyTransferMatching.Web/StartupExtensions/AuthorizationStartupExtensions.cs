using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthorizationStartupExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.ManageAccount, policy =>
                {
                    policy.Requirements.Add(new ManageAccountRequirement());
                });
            });

            return services;
        }
    }
}
