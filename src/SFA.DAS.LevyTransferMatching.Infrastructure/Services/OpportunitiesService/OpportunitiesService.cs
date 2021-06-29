using System.Collections.Generic;
using System.Net;
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

        public async Task<OpportunityDto> GetOpportunity(int id)
        {
            OpportunityDto opportunity = null;

            var response = await _client.GetAsync($"pledges/{id}");

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
    }
}