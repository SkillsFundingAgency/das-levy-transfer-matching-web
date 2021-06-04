using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgesService
{
    public class PledgesService : IPledgesService
    {
        private readonly HttpClient _client;

        public PledgesService(HttpClient client)
        {
            _client = client;
        }

        public async Task PostPledge(PledgeDto pledgeDto)
        {
            var response = await _client.PostAsJsonAsync("pledges", pledgeDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
