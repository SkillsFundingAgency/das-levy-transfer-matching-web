using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.LevyTransferMatching.Infrastructure.Stubs;
using EmployerAccountsApiClient = SFA.DAS.LevyTransferMatching.Infrastructure.Api.EmployerAccountsApiClient;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddEmployerAccountsApiExtensions
    {
        public static IServiceCollection AddEmployerAccountsApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var c = configuration.GetSection<EmployerAccountsApiClientConfiguration>();

            var useStub = configuration.GetValue<bool>("UseEmployerAccountApiStub");

            if (environment.IsDevelopment() && useStub)
            {
                services.AddSingleton<IEmployerAccountsApiClient, StubEmployerAccountsApiClient>();
            }
            else
            {
                services.AddSingleton<IEmployerAccountsApiClient, EmployerAccountsApiClient>();
                services.AddHttpClient<EmployerAccountsApiClient>();
            }
          
           return services;
        }
    }
}
