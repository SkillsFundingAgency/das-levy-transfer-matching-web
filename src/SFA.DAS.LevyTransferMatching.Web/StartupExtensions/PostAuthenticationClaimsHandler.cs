using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using SFA.DAS.Encoding;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public class PostAuthenticationClaimsHandler : ICustomClaims
    {
        private readonly IAccountUserService _accountUserService;
        private readonly Infrastructure.Configuration.FeatureToggles _configuration;
        private readonly IEncodingService _encodingService;

        public PostAuthenticationClaimsHandler(IAccountUserService accountUserService, Infrastructure.Configuration.FeatureToggles configuration, IEncodingService encodingService)
        {
            _accountUserService = accountUserService;
            _configuration = configuration;
            _encodingService = encodingService;
        }
        
        public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            var userId = tokenValidatedContext.Principal.Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimIdentifierConfiguration.Id))?
                .Value;
            var email = string.Empty;

            if (_configuration.UseGovSignIn)
            {
                userId = tokenValidatedContext.Principal.Claims
                    .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                    .Value;
                email = tokenValidatedContext.Principal.Claims
                    .First(c => c.Type.Equals(ClaimTypes.Email))
                    .Value;
            }

            var accountInformation = await _accountUserService.GetUserAccounts(email, userId);

            var claims = new List<Claim>();

            if (!accountInformation.UserAccounts.Any())
            {
                return claims;
            }
            
            claims.AddRange(GetAccountClaims(accountInformation, c =>
                    c.Role.Equals(UserRole.Owner.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    c.Role.Equals(UserRole.Transactor.ToString(), StringComparison.CurrentCultureIgnoreCase),
                ClaimIdentifierConfiguration.AccountOwner));
            
            claims.AddRange(GetAccountClaims(accountInformation, c =>
                    c.Role.Equals(UserRole.Transactor.ToString(), StringComparison.CurrentCultureIgnoreCase),
                ClaimIdentifierConfiguration.AccountTransactor));
            
            claims.AddRange(GetAccountClaims(accountInformation, c =>
                    c.Role.Equals(UserRole.Viewer.ToString(), StringComparison.CurrentCultureIgnoreCase),
                ClaimIdentifierConfiguration.AccountViewer));
            
            claims.Add(new Claim(ClaimIdentifierConfiguration.Id, accountInformation.EmployerUserId));
            claims.Add(new Claim(ClaimIdentifierConfiguration.DisplayName, $"{accountInformation.FirstName} {accountInformation.LastName}"));

            if (accountInformation.IsSuspended)
            {
                claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
            }
            
            return claims;
        }

        private IEnumerable<Claim> GetAccountClaims(GetUserAccountsResponse accountInformation, Func<EmployerIdentifier, bool> predicate, string claimName)
        {
            return accountInformation.UserAccounts
                .Where(predicate)
                .Select(c => new Claim(claimName, _encodingService.Decode(c.AccountId, EncodingType.AccountId).ToString())).ToList();
        }
    }
}