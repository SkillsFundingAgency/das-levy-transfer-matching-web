using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;

public class PledgeService(HttpClient client) : IPledgeService
{
    public async Task<long> PostPledge(CreatePledgeRequest request, long accountId)
    {
        var json = JsonConvert.SerializeObject(request, new StringEnumConverter());
        var response = await client.PostAsync($"accounts/{accountId}/pledges/create", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var id = (string)JObject.Parse(await response.Content.ReadAsStringAsync())["id"];

        return long.Parse(id);
    }

    public async Task RejectApplications(SetRejectApplicationsRequest request, long accountId, int pledgeId)
    {
        var json = JsonConvert.SerializeObject(request, new StringEnumConverter());
        var response = await client.PostAsync($"accounts/{accountId}/pledges/{pledgeId}/reject-applications", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetPledgesResponse> GetPledges(long accountId, int? page = 1, int? pageSize = null)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetPledgesResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetCreateResponse> GetCreate(long accountId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/create");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetCreateResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetAmountResponse> GetAmount(string encodedAccountId)
    {
        var response = await client.GetAsync($"accounts/{encodedAccountId}/pledges/create/amount");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetAmountResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetOrganisationNameResponse> GetOrganisationName(string encodedAccountId)
    {
        var response = await client.GetAsync($"accounts/{encodedAccountId}/pledges/create/organisation");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetOrganisationNameResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetSectorResponse> GetSector(long accountId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/create/sector");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetSectorResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetJobRoleResponse> GetJobRole(long accountId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/create/job-role");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetJobRoleResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetLevelResponse> GetLevel(long accountId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/create/level");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetLevelResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetApplicationsResponse> GetApplications(long accountId, int pledgeId, SortColumn? sortOrder, SortOrder? sortDirection)
    {
        var sort = GetApplicationsSortParameters(sortOrder, sortDirection);
        var response = await client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications{sort}");
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<GetApplicationsResponse>(await response.Content.ReadAsStringAsync());
    }

    private static string GetApplicationsSortParameters(SortColumn? sortColumn, SortOrder? sortDirection)
    {
        if (sortColumn.HasValue && sortDirection.HasValue && sortColumn != SortColumn.Default)
        {
            return $"?sortOrder={sortColumn.Value}&sortDirection={sortDirection.Value}";
        }

        return $"?sortOrder=status&sortDirection=ascending";
    }

    public async Task<GetRejectApplicationsResponse> GetRejectApplications(long accountId, int pledgeId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/reject-applications");
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<GetRejectApplicationsResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task<GetApplicationResponse> GetApplication(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}", cancellationToken);
        GetApplicationResponse applicationResponse = null;

        if (response.IsSuccessStatusCode)
        {
            applicationResponse = JsonConvert.DeserializeObject<GetApplicationResponse>(await response.Content.ReadAsStringAsync());
        }
        else if (response.StatusCode != HttpStatusCode.NotFound)
        {
            response.EnsureSuccessStatusCode();
        }

        return applicationResponse;
    }

    public async Task SetApplicationOutcome(long accountId, int applicationId, int pledgeId, SetApplicationOutcomeRequest outcomeRequest)
    {
        var json = JsonConvert.SerializeObject(outcomeRequest, new StringEnumConverter());
        var response = await client.PostAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetApplicationApprovedResponse> GetApplicationApproved(long accountId, int pledgeId, int applicationId)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}/approved");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetApplicationApprovedResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task ClosePledge(long accountId, int pledgeId, ClosePledgeRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var response = await client.PostAsync($"accounts/{accountId}/pledges/{pledgeId}/close", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public async Task<GetApplicationApprovalOptionsResponse> GetApplicationApprovalOptions(long accountId, int pledgeId, int applicationId, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}/approval-options", cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<GetApplicationApprovalOptionsResponse>(await response.Content.ReadAsStringAsync());
    }

    public async Task SetApplicationApprovalOptions(long accountId, int applicationId, int pledgeId, SetApplicationApprovalOptionsRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var response = await client.PostAsync($"accounts/{accountId}/pledges/{pledgeId}/applications/{applicationId}/approval-options", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }
}