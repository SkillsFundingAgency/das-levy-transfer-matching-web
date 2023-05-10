using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions
{
    public class PostAuthenticationClaimsHandler : ICustomClaims
    {
        private readonly IAccountUserService _accountUserService;
        private readonly Infrastructure.Configuration.FeatureToggles _configuration;

        public PostAuthenticationClaimsHandler(IAccountUserService accountUserService, Infrastructure.Configuration.FeatureToggles configuration)
        {
            _accountUserService = accountUserService;
            _configuration = configuration;
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

            if (!accountInformation.EmployerAccounts.Any())
            {
                return claims;
            }
            var accountsAsJson = JsonConvert.SerializeObject(accountInformation.EmployerAccounts.ToDictionary(k => k.AccountId));
            var associatedAccountsClaim = new Claim(ClaimIdentifierConfiguration.Account, accountsAsJson, JsonClaimValueTypes.Json);
            claims.Add(associatedAccountsClaim);
            claims.Add(new Claim(ClaimIdentifierConfiguration.Id, accountInformation.EmployerUserId));
            claims.Add(new Claim(ClaimIdentifierConfiguration.DisplayName, $"{accountInformation.FirstName} {accountInformation.LastName}"));

            if (accountInformation.IsSuspended)
            {
                claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
            }
            
            return claims;
        }

    }
}