using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;

public interface IAccountUserService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}

public class AccountUserService(HttpClient httpClient) : IAccountUserService, IGovAuthEmployerAccountService
{
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var response = await httpClient.GetAsync($"AccountUsers/{userId}/accounts?email={WebUtility.UrlEncode(email)}");

        if (!response.IsSuccessStatusCode)
        {
            return new EmployerUserAccounts();
        }

        var result = JsonConvert.DeserializeObject<GetUserAccountsResponse>(await response.Content.ReadAsStringAsync());
        
        return new EmployerUserAccounts
        {
            EmployerAccounts = result.UserAccounts != null
                ? result.UserAccounts.Select(c => new EmployerUserAccountItem
                {
                    Role = c.Role,
                    AccountId = c.AccountId,
                    ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                    EmployerName = c.EmployerName,
                }).ToList()
                : [],
            FirstName = result.FirstName,
            IsSuspended = result.IsSuspended,
            LastName = result.LastName,
            EmployerUserId = result.EmployerUserId,
        };
    }
}