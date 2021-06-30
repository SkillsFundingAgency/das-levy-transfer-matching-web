using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var useStub = configuration.GetValue<bool>("UseStubEmployerAuthorisation");

            if (useStub && environment.IsDevelopment())
            {
                services.AddAuthentication("Employer-stub").AddScheme<AuthenticationSchemeOptions, EmployerStubAuthenticationHandler>(
                    "Employer-stub",
                    options => { });
            }
            else
            {
                AddEmployerAuthentication(services, configuration, environment);
            }
        }

        public static void AddEmployerAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var authConfig = configuration.GetSection<Infrastructure.Configuration.Authentication>();

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

                }).AddOpenIdConnect(options =>
                {
                    options.ClientId = authConfig.ClientId;
                    options.ClientSecret = authConfig.ClientSecret;
                    options.Authority = authConfig.BaseAddress;
                    options.UsePkce = authConfig.UsePkce;
                    options.ResponseType = OpenIdConnectResponseType.Code;

                    var scopes = authConfig.Scopes.Split(' ');

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
                    options.Events.OnTokenValidated = async (ctx) => await PopulateAccountsClaims(ctx);
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
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx)
        {
            var userIdString = ctx.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaimTypes.UserId))
                .Value;

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var claims = await userService.GetClaims(userId);

                claims.ToList().ForEach(c => ctx.Principal.Identities.First().AddClaim(c));
            }
        }
    }
}
