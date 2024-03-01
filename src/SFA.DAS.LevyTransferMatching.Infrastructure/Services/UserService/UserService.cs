using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

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

        public bool IsUserChangeAuthorized(string accountId)
        {
            var result = TryGetUserClaimValue(ClaimIdentifierConfiguration.Account, out var employerAccountClaim);
            if (!result)
            {
                return false;
            }
            
            try
            {
                var employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim);
                var tryParse = Enum.TryParse<UserRole>(employerAccounts[accountId].Role, true, out var userRole);

                if (!tryParse)
                {
                    return false;
                }

                if (userRole is UserRole.Owner || userRole is UserRole.Transactor)
                {
                    return true;
                }
            }
            catch (JsonSerializationException)
            {
                return false;
            }

            return false;
        }

        public IEnumerable<string> GetUserOwnerTransactorAccountIds()
        {
            var result = TryGetUserClaimValue(ClaimIdentifierConfiguration.Account, out var employerAccountClaim);
            if (!result)
            {
                return null;
            }
            try
            {
                var employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim);

                return employerAccounts.Values
                    .Where(c => c.Role.Equals("Owner", StringComparison.CurrentCultureIgnoreCase) ||
                                c.Role.Equals("Transactor", StringComparison.CurrentCultureIgnoreCase))
                    .Select(c => c.AccountId);

            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        public bool IsOwnerOrTransactor(string accountId)
        {
            return IsUserChangeAuthorized(accountId);
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