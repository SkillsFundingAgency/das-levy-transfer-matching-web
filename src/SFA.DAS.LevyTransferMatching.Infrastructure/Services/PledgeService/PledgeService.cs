using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public class PledgeService : IPledgeService
    {
        private readonly HttpClient _client;

        public PledgeService(HttpClient client)
        {
            _client = client;
        }

        public async Task<long> PostPledge(PledgeDto pledgeDto, long accountId)
        {
            var json = JsonConvert.SerializeObject(pledgeDto, new StringEnumConverter());
            var response = await _client.PostAsync($"accounts/{accountId}/pledges", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var id = (string)JObject.Parse(await response.Content.ReadAsStringAsync())["id"];

            return long.Parse(id);
        }

        public async Task<GetMyPledgesResponse> GetMyPledges(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/my-pledges");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetMyPledgesResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetCreateResponse> GetCreate(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetCreateResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetAmountResponse> GetAmount(string accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/amount");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetAmountResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetSectorResponse> GetSector(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/sector");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetJobRoleResponse> GetJobRole(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/job-role");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetJobRoleResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetLevelResponse> GetLevel(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/level");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetLevelResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}