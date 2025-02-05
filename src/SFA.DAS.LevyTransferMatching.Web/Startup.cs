using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.EmployerUrlHelper.DependencyResolution;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.FeatureToggles;
using SFA.DAS.LevyTransferMatching.Web.Filters;
using SFA.DAS.LevyTransferMatching.Web.ModelBinders;
using SFA.DAS.LevyTransferMatching.Web.StartupExtensions;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web;

public class Startup
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddConfigurationOptions(_configuration);

        var config = _configuration.GetSection<LevyTransferMatchingWeb>();

        services.AddSingleton(config);
        services.AddSingleton(_configuration.GetSection<LevyTransferMatchingApi>());

        services.AddControllersWithViews();

        services.AddMvc(options =>
            {
                options.AddValidation();
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new HideAccountNavigationAttribute(false));
                options.Filters.Add(new EnableGoogleAnalyticsAttribute(_configuration.GetSection<GoogleAnalytics>()));
                options.Filters.Add(new SetZenDeskValuesAttribute(_configuration.GetSection<ZenDesk>()));

                options.Filters.Add(new GoogleAnalyticsFilter());

                if (!config.IsLive)
                {
                    options.Filters.Add<DisabledActionFilter>();
                }

                options.ModelBinderProviders.Insert(0, new AutoDecodeModelBinderProvider());
            })
            .AddControllersAsServices()
            .SetDefaultNavigationSection(NavigationSection.AccountsFinance);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Startup>();

        services.AddEmployerAuthentication(_configuration);
        services.AddAuthorizationPolicies();
        services.AddCache(_environment, config);
        services.AddMemoryCache();
        services.AddCookieTempDataProvider();
        services.AddDasDataProtection(config, _environment);
        services.AddDasHealthChecks(config);
        services.AddEncodingService();
        services.AddHttpContextAccessor();
        services.AddServiceRegistrations();
        services.AddEmployerSharedUi(_configuration);
        services.AddEmployerUrlHelper();
        services.AddAsyncValidators();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddApplicationInsightsTelemetry();

#if DEBUG
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
#endif
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseDasHealthChecks();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
    }
}