using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Api;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public static class AddServiceRegistrationExtensions
    {
        public static void AddServiceRegistrations(this IServiceCollection services)
        {
            services.AddHttpClient<IApiClient, ApiClient>();
        }
    }
}
