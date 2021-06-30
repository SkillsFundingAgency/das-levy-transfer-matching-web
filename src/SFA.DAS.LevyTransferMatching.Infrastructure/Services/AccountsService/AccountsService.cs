using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService
{
    public class AccountsService : IAccountsService
    {
        private readonly HttpClient _client;

        public AccountsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AccountDto> GetAccountDetail(string encodedAccountId)
        {
            var response = await _client.GetAsync($"accounts/{encodedAccountId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accountDto = JsonConvert.DeserializeObject<AccountDto>(content);
            accountDto.RemainingTransferAllowance = Convert.ToInt32(Math.Round(accountDto.RemainingTransferAllowance, MidpointRounding.AwayFromZero));

            return accountDto;
        }
    }
}
