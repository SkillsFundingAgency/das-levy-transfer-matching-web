using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthorizationStartupExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.IsAuthenticated, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ClaimIdentifierConfiguration.Id);
                    policy.Requirements.Add(new AccountActiveRequirement());
                });
                options.AddPolicy(PolicyNames.ManageAccount, policy =>
                {
                    policy.Requirements.Add(new ManageAccountRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                });
                options.AddPolicy(PolicyNames.ViewAccount, policy =>
                {
                    policy.Requirements.Add(new ViewAccountRequirement());
                    policy.Requirements.Add(new AccountActiveRequirement());
                });
            });

            return services;
        }
    }

    //TODO once upgraded to .net6 - this filter can be deleted
    public class AccountActiveFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;

        public AccountActiveFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null)
            {
                var isAccountSuspended = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value;
                if (isAccountSuspended != null && isAccountSuspended.Equals("Suspended", StringComparison.CurrentCultureIgnoreCase))
                {
                    context.HttpContext.Response.Redirect(RedirectExtension.GetAccountSuspendedRedirectUrl(_configuration["ResourceEnvironmentName"]));
                }
            }
        }
    }
}
