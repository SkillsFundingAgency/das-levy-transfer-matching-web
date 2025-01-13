using System.Net.Http;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.Http;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Services;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddServiceRegistrationExtensions
{
    public static void AddServiceRegistrations(this IServiceCollection services)
    {
        services.AddHttpClient<IApiClient, ApiClient>();

        services.AddSingleton<IAuthorizationHandler, ManageAccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ViewAccountAuthorizationHandler>();
        services.AddTransient<IEmployerAccountAuthorizationHandler, EmployerAccountAuthorizationHandler>();
            
        services.AddTransient<ILocationValidatorService, LocationValidatorService>();
        services.AddTransient<ICacheStorageService, CacheStorageService>();
        services.AddTransient<IPledgeOrchestrator, PledgeOrchestrator>();
        services.AddTransient<IOpportunitiesOrchestrator, OpportunitiesOrchestrator>();
        services.AddTransient<ILocationOrchestrator, LocationOrchestrator>();
        services.AddTransient<IApplicationsOrchestrator, ApplicationsOrchestrator>();
        services.AddTransient<ICreatePledgeOrchestrator, CreatePledgeOrchestrator>();
        services.AddTransient<ICsvHelperService, CsvHelperService>();
        services.AddTransient<IUserService, UserService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
            
        services.AddClient<IPledgeService>((c, s) => new PledgeService(c));
        services.AddClient<IOpportunitiesService>((c, s) => new OpportunitiesService(c));
        services.AddClient<ILocationService>((c, s) => new LocationService(c));
        services.AddClient<IApplicationsService>((c, s) => new ApplicationsService(c));
        services.AddClient<IAccountUserService>((c, s) => new AccountUserService(c));
        services.AddClient<IGovAuthEmployerAccountService>((c, s) => new AccountUserService(c));
    }

    private static IServiceCollection AddClient<T>(this IServiceCollection serviceCollection, Func<HttpClient, IServiceProvider, T> instance) where T : class
    {
        serviceCollection.AddTransient(s =>
        {
            var settings = s.GetService<LevyTransferMatchingApi>();

            var clientBuilder = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithApimAuthorisationHeader(settings)
                .WithLogging(s.GetService<ILoggerFactory>());

            var httpClient = clientBuilder.Build();

            if (!settings.ApiBaseUrl.EndsWith("/"))
            {
                httpClient.BaseAddress = new Uri(settings.ApiBaseUrl + "/");
            }
            else
            {
                httpClient.BaseAddress = new Uri(settings.ApiBaseUrl);
            }

            return instance.Invoke(httpClient, s);
        });

        return serviceCollection;
    }
}