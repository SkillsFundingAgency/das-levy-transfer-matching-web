﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Authentication;
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
}
