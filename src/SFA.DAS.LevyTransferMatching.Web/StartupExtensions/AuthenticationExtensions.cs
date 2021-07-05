using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthenticationExtensions
    {
        public static void AddEmployerAuthentication(this IServiceCollection services, Infrastructure.Configuration.Authentication configuration)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

                }).AddOpenIdConnect(options =>
                {
                    options.ClientId = configuration.ClientId;
                    options.ClientSecret = configuration.ClientSecret;
                    options.Authority = configuration.BaseAddress;
                    options.UsePkce = configuration.UsePkce;
                    options.ResponseType = configuration.ResponseType;

                    var scopes = configuration.Scopes.Split(' ');
                    
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.ClaimActions.MapUniqueJsonKey("sub", "id");
                    options.Events.OnRemoteFailure = c =>
                    {
                        if (c.Failure.Message.Contains("Correlation failed"))
                        {
                            c.Response.Redirect("/");
                            c.HandleResponse();
                        }

                        return Task.CompletedTask;
                    };
                })
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = "SFA.DAS.LevyTransferMatching.Web.Auth";
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.CookieManager = new ChunkingCookieManager() { ChunkSize = 3000 };
                });

            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IEmployerAccountsService, ILogger<Startup>>((options, accountsService, logger) =>
                {
                    options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaim(ctx, accountsService, logger);
                });
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IEmployerAccountsService accountsService, ILogger<Startup> logger)
        {
            var userId = ctx.Principal.Claims
                .First(c => c.Type.Equals(ClaimIdentifierConfiguration.Id))
                .Value;

            logger.LogInformation($"Populating Accounts claims for user {userId}");

            if (Guid.TryParse(userId, out Guid userRef))
            {
                var roles = new HashSet<UserRole> { UserRole.Owner, UserRole.Transactor };
                var userAccounts = await accountsService.GetUserAccounts(userRef, roles, CancellationToken.None);

                foreach (var userAccount in userAccounts)
                {
                    logger.LogInformation($"Adding claim for account {userAccount} for user {userId}");
                    var claim = new Claim(ClaimIdentifierConfiguration.Account, userAccount.ToString());
                    ctx.Principal.Identities.First().AddClaim(claim);
                }
            }
        }
    }
}
