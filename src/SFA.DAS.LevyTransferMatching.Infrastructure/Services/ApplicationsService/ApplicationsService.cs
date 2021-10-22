using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService
{
    public class ApplicationsService : IApplicationsService
    {
        private readonly HttpClient _httpClient;

        public ApplicationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetApplicationsResponse> GetApplications(long accountId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications", cancellationToken);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<GetApplicationsResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Types.GetApplicationResponse> GetApplication(long accountId, int applicationId)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications/{applicationId}");

            Types.GetApplicationResponse getApplicationResponse = null; 
            
            if (response.IsSuccessStatusCode)
            {
                getApplicationResponse = JsonConvert.DeserializeObject<Types.GetApplicationResponse>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.NotFound)
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            return getApplicationResponse;
        }
    }
}