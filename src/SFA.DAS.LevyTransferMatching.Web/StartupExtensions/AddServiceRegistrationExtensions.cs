using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System;
using System.Net.Http;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddServiceRegistrationExtensions
    {
        public static void AddServiceRegistrations(this IServiceCollection services)
        {
            services.AddHttpClient<IApiClient, ApiClient>();

            services.AddSingleton<IDocumentClientFactory, DocumentClientFactory>();
            services.AddTransient<IAccountUsersReadOnlyRepository, AccountUsersReadOnlyRepository>();

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IAuthorizationContextProvider, AuthorizationContextProvider>();

            services.AddTransient<ICacheStorageService, CacheStorageService>();
            services.AddTransient<IPledgeOrchestrator, PledgeOrchestrator>();

            services.AddClient<IAccountsService, LevyTransferMatchingApi>((c, s) => new AccountsService(c));
            services.AddClient<ILocationService, FindApprenticeshipTrainingApiConfiguration>((c, s) => new LocationService(c));
        }

        private static IServiceCollection AddClient<T, TConfig>(
            this IServiceCollection serviceCollection,
            Func<HttpClient, IServiceProvider, T> instance) where T : class where TConfig : IApimClientConfiguration
        {
            serviceCollection.AddTransient(s =>
            {
                var settings = s.GetService<TConfig>();

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
