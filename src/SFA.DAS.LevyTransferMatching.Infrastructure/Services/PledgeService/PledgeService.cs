using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
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

        public async Task<long> PostPledge(CreatePledgeRequest request, long accountId)
        {
            var json = JsonConvert.SerializeObject(request, new StringEnumConverter());
            var response = await _client.PostAsync($"accounts/{accountId}/pledges/create", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var id = (string)JObject.Parse(await response.Content.ReadAsStringAsync())["id"];

            return long.Parse(id);
        }

        public async Task<GetPledgesResponse> GetPledges(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges");

            return await response.HandleDeserialisationOrThrow<GetPledgesResponse>();
        }

        public async Task<GetCreateResponse> GetCreate(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create");

            return await response.HandleDeserialisationOrThrow<GetCreateResponse>();
        }

        public async Task<GetAmountResponse> GetAmount(string encodedAccountId)
        {
            var response = await _client.GetAsync($"accounts/{encodedAccountId}/pledges/create/amount");

            return await response.HandleDeserialisationOrThrow<GetAmountResponse>();
        }

        public async Task<GetSectorResponse> GetSector(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/sector");

            return await response.HandleDeserialisationOrThrow<GetSectorResponse>();
        }

        public async Task<GetJobRoleResponse> GetJobRole(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/job-role");
            return await response.HandleDeserialisationOrThrow<GetJobRoleResponse>();
        }

        public async Task<GetLevelResponse> GetLevel(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create/level");

            return await response.HandleDeserialisationOrThrow<GetLevelResponse>();
        }

        public async Task<GetApplicationsResponse> GetApplications(long accountId, int pledgeId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications");

            return await response.HandleDeserialisationOrThrow<GetApplicationsResponse>();
        }

        public async Task<GetApplicationResponse> GetApplication(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}", cancellationToken);

            return await response.HandleDeserialisationOrThrow<GetApplicationResponse>();
        }

        public async Task SetApplicationOutcome(long accountId, int applicationId, int pledgeId, SetApplicationOutcomeRequest outcomeRequest)
        {
            var json = JsonConvert.SerializeObject(outcomeRequest, new StringEnumConverter());
            var response = await _client.PostAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task<GetApplicationApprovedResponse> GetApplicationApproved(long accountId, int pledgeId, int applicationId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}/approved");
            return await response.HandleDeserialisationOrThrow<GetApplicationApprovedResponse>();
        }
    }
}