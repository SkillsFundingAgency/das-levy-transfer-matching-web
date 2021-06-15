using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.SearchFundingService
{
    public class SearchFundingService : ISearchFundingService
    {
        private readonly HttpClient _client;

        public SearchFundingService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<OpportunityDto>> GetAllOpportunities()
        {
            var response = await _client.GetAsync($"accounts/ABC123/pledges");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<OpportunityDto>>(await response.Content.ReadAsStringAsync());
        }
    }
}
