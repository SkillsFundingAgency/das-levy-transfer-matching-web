﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Extensions;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;

public class OpportunitiesService(HttpClient client) : IOpportunitiesService
{
    public async Task<GetApplyResponse> GetApply(long accountId, int opportunityId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetApplyResponse>(await response.Content.ReadAsStringAsync());
    }

        public async Task<GetIndexResponse> GetIndex(IEnumerable<string> sectors, OpportunitiesSortBy sortBy, int page, int? pageSize)
        {
            var filters = sectors != null ? sectors.ToNameValueCollection("sectors") : new NameValueCollection();
            filters.Add("page", page.ToString());
            filters.Add("sortBy", sortBy.ToString());
            if (pageSize != null)
            {
                filters.Add("pageSize", pageSize.ToString());
            }

        var response = await client.GetAsync($"opportunities{filters.ToQueryString()}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetIndexResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetApplicationDetailsResponse> GetApplicationDetails(long accountId, int id, string standardId = default)
    {
        GetApplicationDetailsResponse applicationDetailsResponse = null;

        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{id}/apply/application-details{(standardId != default ? $"?standardId={standardId}" : string.Empty)}");

        if (response.IsSuccessStatusCode)
        {
            applicationDetailsResponse = JsonConvert.DeserializeObject<GetApplicationDetailsResponse>(await response.Content.ReadAsStringAsync());
        }
        else if (response.StatusCode != HttpStatusCode.NotFound)
        {
            response.EnsureSuccessStatusCode();
        }

        return applicationDetailsResponse;
    }

    public async Task<GetMoreDetailsResponse> GetMoreDetails(long accountId, int pledgeId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/more-details");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetMoreDetailsResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/sector");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetSectorResponse> GetSector(long accountId, int pledgeId, string postcode)
    {
        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/sector?postcode={postcode}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetContactDetailsResponse> GetContactDetails(long accountId, int pledgeId)
    {
        GetContactDetailsResponse getContactDetailsResult = null;

        var response = await client.GetAsync($"accounts/{accountId}/opportunities/{pledgeId}/apply/contact-details");

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
            await client.GetAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply/confirmation");

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<GetConfirmationResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<ApplyResponse> PostApplication(long accountId, int opportunityId, ApplyRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var response = await client.PostAsync($"accounts/{accountId}/opportunities/{opportunityId}/apply",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<ApplyResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetDetailResponse> GetDetail(int opportunityId)
    {
        var response = await client.GetAsync($"opportunities/{opportunityId}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetDetailResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetSelectAccountResponse> GetSelectAccount(int opportunityId, string userId)
    {
        var response = await client.GetAsync($"opportunities/{opportunityId}/select-account?userId={userId}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetSelectAccountResponse>(await response.Content.ReadAsStringAsync());
    }
}