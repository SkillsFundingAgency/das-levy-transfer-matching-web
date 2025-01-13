using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AuthorizationStartupExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.IsAuthenticated, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimIdentifierConfiguration.Id);
                policy.Requirements.Add(new AccountActiveRequirement());
            })
            .AddPolicy(PolicyNames.ManageAccount, policy =>
            {
                policy.Requirements.Add(new ManageAccountRequirement());
                policy.Requirements.Add(new AccountActiveRequirement());
            })
            .AddPolicy(PolicyNames.ViewAccount, policy =>
            {
                policy.Requirements.Add(new ViewAccountRequirement());
                policy.Requirements.Add(new AccountActiveRequirement());
            });

        return services;
    }
}
