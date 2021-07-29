using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
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

        public async Task<List<OpportunityDto>> GetAllOpportunities()
        {
            var response = await _client.GetAsync($"pledges");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<OpportunityDto>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<OpportunityDto> GetOpportunity(int id)
        {
            OpportunityDto opportunity = null;

            var response = await _client.GetAsync($"opportunities/{id}");

            if (response.IsSuccessStatusCode)
            {
                opportunity = JsonConvert.DeserializeObject<OpportunityDto>(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
            }

            return opportunity;
        }

        public async Task<ApplicationDetailsDto> GetApplicationDetails(long accountId, int id)
        {
            ApplicationDetailsDto applicationDetailsResponse = null;

            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{id}/create/application-details");

            if (response.IsSuccessStatusCode)
            {
                applicationDetailsResponse = JsonConvert.DeserializeObject<ApplicationDetailsDto>(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
            }

            return applicationDetailsResponse;
        }

         public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/create/sector");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId, string postcode)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/create/sector?postcode={postcode}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetContactDetailsResponse> GetContactDetails(long accountId, int pledgeId)
        {
            GetContactDetailsResponse getContactDetailsResult = null;

            var response = await _client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/contact-details");

            if (response.IsSuccessStatusCode)
            {
                getContactDetailsResult = JsonConvert.DeserializeObject<GetContactDetailsResponse>(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
            }

            return getContactDetailsResult;
        }
		
		public async Task<GetConfirmationResponse> GetConfirmation(long accountId, int opportunityId)
        {
            var response =
                await _client.GetAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply/confirmation");

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<GetConfirmationResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApplyResponse> PostApplication(long accountId, int opportunityId, ApplyRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var response = await _client.PostAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ApplyResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}