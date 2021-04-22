using SFA.DAS.LevyTransferMatching.Domain.ApiClient;

namespace SFA.DAS.LevyTransferMatching.Domain.EmployerAccounts
{
    public class GetAccountUsersApiRequest : IGetAllApiRequest
    {
        public long AccountId { get; }

        public GetAccountUsersApiRequest(string baseUrl, long accountId)
        {
            AccountId = accountId;
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public string GetAllUrl => $"{BaseUrl}/accounts/{AccountId}/users";
    }
}
