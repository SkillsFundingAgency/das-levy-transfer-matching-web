using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddServiceRegistrationExtensions
    {
        public static void AddServiceRegistrations(this IServiceCollection services)
        {
            services.AddHttpClient<IApiClient, ApiClient>();

            services.AddSingleton<IDocumentClientFactory, DocumentClientFactory>();
            services.AddTransient<IAccountUsersReadOnlyRepository, AccountUsersReadOnlyRepository>();

            services.AddSingleton<IAuthorizationHandler, ManageAccountAuthorizationHandler>();

            services.AddTransient<IEmployerAccountsService, EmployerAccountsService>();
            services.AddTransient<ILocationValidatorService, LocationValidatorService>();
            services.AddTransient<ICacheStorageService, CacheStorageService>();
            services.AddTransient<IPledgeOrchestrator, PledgeOrchestrator>();
            services.AddTransient<IOpportunitiesOrchestrator, OpportunitiesOrchestrator>();
            services.AddTransient<ILocationOrchestrator, LocationOrchestrator>();

            services.AddClient<IAccountsService>((c, s) => new AccountsService(c));
            services.AddClient<IPledgeService>((c, s) => new PledgeService(c));
            services.AddClient<ITagService>((c, s) => new TagService(c, s.GetService<ICacheStorageService>()));
            services.AddClient<IOpportunitiesService>((c, s) => new OpportunitiesService(c));
            services.AddClient<ILocationService>((c, s) => new LocationService(c));
        }

        private static IServiceCollection AddClient<T>(
            this IServiceCollection serviceCollection,
            Func<HttpClient, IServiceProvider, T> instance) where T : class
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
                    httpClient.BaseAddress = new Uri(settings.ApiBaseUrl + "/");
                else
                    httpClient.BaseAddress = new Uri(settings.ApiBaseUrl);

                return instance.Invoke(httpClient, s);
            });

            return serviceCollection;
        }
    }
}
