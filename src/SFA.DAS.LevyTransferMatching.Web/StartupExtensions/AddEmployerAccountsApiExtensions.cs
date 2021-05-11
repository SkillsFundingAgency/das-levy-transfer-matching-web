using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Api.Client;
using EmployerAccountsApiClient = SFA.DAS.LevyTransferMatching.Infrastructure.Api.EmployerAccountsApiClient;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddEmployerAccountsApiExtensions
    {
        public static IServiceCollection AddEmployerAccountsApi(this IServiceCollection services)
        {
            services.AddSingleton<IEmployerAccountsApiClient, EmployerAccountsApiClient>();
            return services;
        }
    }
}
