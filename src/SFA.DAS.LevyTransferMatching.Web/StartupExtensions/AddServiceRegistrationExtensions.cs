using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Http;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService;
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

            services.AddClient<IAccountsService>((c, s) => new AccountsService(c));
            services.AddClient<IPledgesService>((c, s) => new PledgesService(c));
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
                {
                    settings.ApiBaseUrl += "/";
                }
                httpClient.BaseAddress = new Uri(settings.ApiBaseUrl);

                return instance.Invoke(httpClient, s);
            });

            return serviceCollection;
        }
    }
}
