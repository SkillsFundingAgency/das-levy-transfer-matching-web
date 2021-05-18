using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.Context;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Authorization;

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
        }
    }
}
