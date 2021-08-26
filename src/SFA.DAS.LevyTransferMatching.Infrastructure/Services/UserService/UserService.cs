using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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

        public bool IsUserChangeAuthorized()
        {
            return TryGetUserClaimValue(ClaimIdentifierConfiguration.AccountOwner, out _) || TryGetUserClaimValue(ClaimIdentifierConfiguration.AccountTransactor, out _);
        }

        public IEnumerable<long> GetUserAccountIds()
        {
            return GetUserClaimsAsLongs(ClaimIdentifierConfiguration.Account);
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

        private bool TryGetUserClaimValues(string key, out IEnumerable<string> values)
        {
            var claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            var claims = claimsIdentity.FindAll(key);

            var exists = claims != null;

            values = exists ? claims.Select(x => x.Value) : null;

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

        private IEnumerable<long> GetUserClaimsAsLongs(string claim)
        {
            if (IsUserAuthenticated() && TryGetUserClaimValues(claim, out var values))
            {
                return values.Select(long.Parse);
            }
            return null;
        }
    }
}