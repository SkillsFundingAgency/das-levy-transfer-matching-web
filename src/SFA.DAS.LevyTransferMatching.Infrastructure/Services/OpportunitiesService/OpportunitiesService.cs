using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService
{
    public class OpportunitiesService : IOpportunitiesService
    {
        private readonly HttpClient _client;

        public OpportunitiesService(HttpClient client)
        {
            _client = client;
        }

        public async Task<GetApplyResponse> GetApply(long accountId, int opportunityId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply");

            return await response.HandleDeserialisationOrThrow<GetApplyResponse>();
        }

        public async Task<GetIndexResponse> GetIndex()
        {
            var response = await _client.GetAsync($"opportunities");

            return await response.HandleDeserialisationOrThrow<GetIndexResponse>();
        }

        public async Task<GetApplicationDetailsResponse> GetApplicationDetails(long accountId, int id, string standardId = default)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{id}/apply/application-details{(standardId != default ? $"?standardId={standardId}" : string.Empty)}");

            return await response.HandleDeserialisationOrThrow<GetApplicationDetailsResponse>();
        }

        public async Task<GetMoreDetailsResponse> GetMoreDetails(long accountId, int pledgeId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/more-details");

            return await response.HandleDeserialisationOrThrow<GetMoreDetailsResponse>();
        }

        public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/sector");

            return await response.HandleDeserialisationOrThrow<GetSectorResponse>();
        }

        public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId, string postcode)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/sector?postcode={postcode}");

            return await response.HandleDeserialisationOrThrow<GetSectorResponse>();
        }

        public async Task<GetContactDetailsResponse> GetContactDetails(long accountId, int pledgeId)
        {
           
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/contact-details");

            return await response.HandleDeserialisationOrThrow<GetContactDetailsResponse>();
        }
		
		public async Task<GetConfirmationResponse> GetConfirmation(long accountId, int opportunityId)
        {
            var response =
                await _client.GetAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply/confirmation");

            return await response.HandleDeserialisationOrThrow<GetConfirmationResponse>();
        }

        public async Task<ApplyResponse> PostApplication(long accountId, int opportunityId, ApplyRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var response = await _client.PostAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ApplyResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetDetailResponse> GetDetail(int opportunityId)
        {
            var response = await _client.GetAsync($"opportunities/{opportunityId}");
            
            return await response.HandleDeserialisationOrThrow<GetDetailResponse>();
        }

        public async Task<GetSelectAccountResponse> GetSelectAccount(int opportunityId, string userId)
        {
            var response = await _client.GetAsync($"opportunities/{opportunityId}/select-account?userId={userId}");

            return await response.HandleDeserialisationOrThrow<GetSelectAccountResponse>();
        }
    }
}