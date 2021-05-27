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

        public async Task<string> GetTransferAllowance(string encodedAccountId)
        {
            //var response = await _client.GetAsync("");
            //response.EnsureSuccessStatusCode();

            //return await response.Content.ReadAsStringAsync();
            return "";
        }
    }
}
