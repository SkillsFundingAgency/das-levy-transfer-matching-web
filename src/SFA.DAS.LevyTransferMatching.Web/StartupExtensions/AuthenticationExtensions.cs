using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AuthenticationExtensions
    {
        public static void AddEmployerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"] != null 
                && configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                services.Configure<GovUkOidcConfiguration>(configuration.GetSection("GovUkOidcConfiguration"));
                services.AddAndConfigureGovUkAuthentication(configuration, $"{typeof(AuthenticationExtensions).Assembly.GetName().Name}.Auth",typeof(PostAuthenticationClaimsHandler));
            }
            else
            {
                var employerConfig = configuration.GetSection<Infrastructure.Configuration.Authentication>();
                
                services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

                }).AddOpenIdConnect(options =>
                {
                    options.ClientId = employerConfig.ClientId;
                    options.ClientSecret = employerConfig.ClientSecret;
                    options.Authority = employerConfig.BaseAddress;
                    options.UsePkce = employerConfig.UsePkce;
                    options.ResponseType = employerConfig.ResponseType;

                    var scopes = employerConfig.Scopes.Split(' ');
                    
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
                .Configure<ICustomClaims>((options, customClaimsHandler) =>
                {
                    options.Events.OnTokenValidated = async (ctx) =>
                    {
                        var claims = await customClaimsHandler.GetClaims(ctx);
                        ctx.Principal.Identities.First().AddClaims(claims);
                    };
                });
            }
            
        }

        
    }
}
