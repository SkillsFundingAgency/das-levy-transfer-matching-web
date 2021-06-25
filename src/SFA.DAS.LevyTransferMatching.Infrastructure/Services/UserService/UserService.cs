using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AccountDto>> GetUserAccounts(string userId)
        {
            var response = await _httpClient.GetAsync($"users/{userId}/accounts");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<AccountDto>>(await response.Content.ReadAsStringAsync());
        }
    }
}