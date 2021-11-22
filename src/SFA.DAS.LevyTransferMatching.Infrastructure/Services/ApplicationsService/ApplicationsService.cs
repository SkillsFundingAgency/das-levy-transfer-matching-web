using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;

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

        public async Task<GetApplicationResponse> GetApplication(long accountId, int applicationId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications/{applicationId}", cancellationToken);

            return await response.HandleDeserialisationOrThrow<Types.GetApplicationResponse>();
        }

        public async Task SetApplicationAcceptance(SetApplicationAcceptanceRequest request, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(request);
            var response = await _httpClient.PostAsync($"accounts/{request.AccountId}/applications/{request.ApplicationId}",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"), cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task<GetAcceptedResponse> GetAccepted(long accountId, int applicationId)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications/{applicationId}/accepted");

            return await response.HandleDeserialisationOrThrow<GetAcceptedResponse>();
        }
        public async Task<GetDeclinedResponse> GetDeclined(long accountId, int applicationId)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications/{applicationId}/declined");

            GetDeclinedResponse getDeclinedResponse = null;
            if (response.IsSuccessStatusCode)
            {
                getDeclinedResponse = JsonConvert.DeserializeObject<GetDeclinedResponse>(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
            }

            return getDeclinedResponse;
        }

        public async Task<GetWithdrawnResponse> GetWithdrawn(long accountId, int applicationId)
        {
            var response = await _httpClient.GetAsync($"accounts/{accountId}/applications/{applicationId}/withdrawn");

            GetWithdrawnResponse getWithdrawnResponse = null;
            if (response.IsSuccessStatusCode)
            {
                getWithdrawnResponse = JsonConvert.DeserializeObject<GetWithdrawnResponse>(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
            }

            return getWithdrawnResponse;
        }
    }
}