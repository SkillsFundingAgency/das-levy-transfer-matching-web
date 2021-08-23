using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

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

        public string GetUserId()
        {
            return GetUserClaimAsString(ClaimIdentifierConfiguration.Id);
        }

        public string GetUserDisplayName()
        {
            return GetUserClaimAsString(ClaimIdentifierConfiguration.DisplayName);
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