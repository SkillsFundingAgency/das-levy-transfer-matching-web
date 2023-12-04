using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;
using SFA.DAS.LevyTransferMatching.Web.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public interface IEmployerAccountAuthorizationHandler
    {
        Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context, UserRole minimumAllowedRole);
    }
    public class EmployerAccountAuthorizationHandler: IEmployerAccountAuthorizationHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountUserService _accountsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployerAccountAuthorizationHandler> _logger;

        public EmployerAccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IAccountUserService accountsService, IConfiguration configuration, ILogger<EmployerAccountAuthorizationHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountsService = accountsService;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context, UserRole minimumAllowedRole)
        {
            if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.EncodedAccountId))
            {
                return false;
            }
            var accountIdFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.EncodedAccountId].ToString().ToUpper();
            var employerAccountClaim = context.User.FindFirst(c=>c.Type.Equals(ClaimIdentifierConfiguration.Account));

            if(employerAccountClaim?.Value == null)
                return false;

            Dictionary<string, EmployerUserAccountItem> employerAccounts;

            try
            {
                employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim.Value);
            }
            catch (JsonSerializationException e)
            {
                _logger.LogError(e, "Could not deserialize employer account claim for user");
                return false;
            }

            EmployerUserAccountItem employerIdentifier = null;

            if (employerAccounts != null)
            {
                employerIdentifier = employerAccounts.ContainsKey(accountIdFromUrl) 
                    ? employerAccounts[accountIdFromUrl] : null;
            }

            if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
            {
                string requiredIdClaim = ClaimIdentifierConfiguration.Id;

                if (_configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"] != null
                    && _configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"]
                        .Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    requiredIdClaim = ClaimTypes.NameIdentifier;
                }
                
                if (!context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)))
                    return false;
                
                var userClaim = context.User.Claims
                    .First(c => c.Type.Equals(requiredIdClaim));

                var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;

                var userId = userClaim.Value;

                var result = _accountsService.GetUserAccounts(email, userId).Result;
                
                var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
                var associatedAccountsClaim = new Claim(ClaimIdentifierConfiguration.Account, accountsAsJson, JsonClaimValueTypes.Json);
                
                var updatedEmployerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(associatedAccountsClaim.Value);

                userClaim.Subject.AddClaim(associatedAccountsClaim);
                
                if (!updatedEmployerAccounts.ContainsKey(accountIdFromUrl))
                {
                    return false;
                }
                employerIdentifier = updatedEmployerAccounts[accountIdFromUrl];
            }
            
            return CheckUserRoleForAccess(employerIdentifier, minimumAllowedRole);
        }

        private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier, UserRole minimumAllowedRole)
        {
            bool tryParse = Enum.TryParse<UserRole>(employerIdentifier.Role, true, out var userRole);

            if (!tryParse)
            {
                return false;
            }

            return minimumAllowedRole switch
            {
                UserRole.Owner => userRole is UserRole.Owner,
                UserRole.Transactor => userRole is UserRole.Owner || userRole is UserRole.Transactor,
                UserRole.Viewer => userRole is UserRole.Owner || userRole is UserRole.Transactor || userRole is UserRole.Viewer,
                _ => false
            };
        }
    }
}