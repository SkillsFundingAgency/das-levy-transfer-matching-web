using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.LevyTransferMatching.Infrastructure.Stubs;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddEmployerAccountsApiExtensions
    {
        public static IServiceCollection AddEmployerAccountsApi(this IServiceCollection services)
        {
            services.AddSingleton<IEmployerAccountsApiClient, StubEmployerAccountsApiClient>();
            return services;
        }
    }
}
