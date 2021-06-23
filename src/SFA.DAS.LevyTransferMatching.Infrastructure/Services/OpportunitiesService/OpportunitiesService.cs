using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

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

        public async Task<OpportunityDto> GetOpportunity(string encodedId)
        {
            var response = await _client.GetAsync($"pledges/{encodedId}");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<OpportunityDto>(await response.Content.ReadAsStringAsync());
        }
    }
}
