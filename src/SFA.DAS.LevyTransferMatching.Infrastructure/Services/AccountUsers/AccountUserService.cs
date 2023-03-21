using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers
{
    public class AccountUserService : IAccountUserService
    {
        private readonly HttpClient _httpClient;

        public AccountUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetUserAccountsResponse> GetUserAccounts(string email, string userId)
        {
            var response = await _httpClient.GetAsync($"AccountUsers/{userId}/accounts?{HttpUtility.UrlEncode(email)}");

            if (!response.IsSuccessStatusCode)
            {
                return new GetUserAccountsResponse();    
            }

            return JsonConvert.DeserializeObject<GetUserAccountsResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}