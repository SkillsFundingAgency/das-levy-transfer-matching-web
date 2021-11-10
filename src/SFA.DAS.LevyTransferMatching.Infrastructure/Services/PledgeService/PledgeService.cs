﻿using System;
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
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetPledgesResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetCreateResponse> GetCreate(long accountId)
        {
            var response = await _client.GetAsync($"accounts/{accountId}/pledges/create");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GetCreateResponse>(await response.Content.ReadAsStringAsync());
        }

        public async Task<GetAmountResponse> GetAmount(string encodedAccountId)
        {
            var response = await _client.GetAsync($"accounts/{encodedAccountId}/pledges/create/amount");
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