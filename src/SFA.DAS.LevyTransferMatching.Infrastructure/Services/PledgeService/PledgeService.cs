using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public class PledgeService : IPledgeService
    {
        private readonly HttpClient _client;

        public PledgeService(HttpClient client)
        {
            _client = client;
        }

        public async Task PostPledge(PledgeDto pledgeDto, string encodedAccountId)
        {
            var json = JsonConvert.SerializeObject(pledgeDto, new StringEnumConverter());
            var response = await _client.PostAsync($"accounts/{encodedAccountId}/pledges", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
