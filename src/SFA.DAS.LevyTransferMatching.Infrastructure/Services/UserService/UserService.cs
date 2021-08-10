using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public UserService(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public string UserId => GetUserClaimAsString(ClaimIdentifierConfiguration.Id);

        public async Task<IEnumerable<UserAccountDto>> GetLoggedInUserAccounts()
        {
            var response = await _httpClient.GetAsync($"users/{UserId}/accounts");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<UserAccountDto>>(await response.Content.ReadAsStringAsync());
        }

        public bool IsUserChangeAuthorized()
        {
            return TryGetUserClaimValue(ClaimIdentifierConfiguration.AccountOwner, out _) || TryGetUserClaimValue(ClaimIdentifierConfiguration.AccountTransactor, out _);
        }

        private bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        private bool TryGetUserClaimValue(string key, out string value)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var claim = claimsIdentity.FindFirst(key);
            var exists = claim != null;

            value = exists ? claim.Value : null;

            return exists;
        }

        private string GetUserClaimAsString(string claim)
        {
            if (IsUserAuthenticated() && TryGetUserClaimValue(claim, out var value))
            {
                return value;
            }
            return null;
        }
    }
}