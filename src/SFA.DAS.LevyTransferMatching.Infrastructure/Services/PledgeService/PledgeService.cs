using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public class PledgeService : IPledgeService
    {
        private readonly HttpClient _client;
        private readonly IEncodingService _encodingService;

        public PledgeService(HttpClient client, IEncodingService encodingService)
        {
            _client = client;
            _encodingService = encodingService;
        }

        public async Task<string> PostPledge(PledgeDto pledgeDto, long accountId)
        {
            var json = JsonConvert.SerializeObject(pledgeDto, new StringEnumConverter());
            var response = await _client.PostAsync($"accounts/{accountId}/pledges", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<PledgeDto>(await response.Content.ReadAsStringAsync());

            return _encodingService.Encode(long.Parse(result.Id), EncodingType.PledgeId);
        }
    }
}
