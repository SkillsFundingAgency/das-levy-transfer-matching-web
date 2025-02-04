using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
using SFA.DAS.LevyTransferMatching.Web.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication;

public interface IEmployerAccountAuthorizationHandler
{
    Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context, UserRole minimumAllowedRole);
}

public class EmployerAccountAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IAccountUserService accountsService,
    IConfiguration configuration,
    ILogger<EmployerAccountAuthorizationHandler> logger)
    : IEmployerAccountAuthorizationHandler
{
    public Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context, UserRole minimumAllowedRole)
    {
        if (!httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue(RouteValueKeys.EncodedAccountId, out var routeValue))
        {
            return Task.FromResult(false);
        }

        var accountIdFromUrl = routeValue.ToString().ToUpper();
        var employerAccountClaim = context.User.FindFirst(c => c.Type.Equals(ClaimIdentifierConfiguration.Account));

        if (employerAccountClaim?.Value == null)
        {
            return Task.FromResult(false);
        }

        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim.Value);
        }
        catch (JsonSerializationException e)
        {
            logger.LogError(e, "Could not deserialize employer account claim for user");
            return Task.FromResult(false);
        }

        EmployerUserAccountItem employerIdentifier = null;

        if (employerAccounts != null)
        {
            employerIdentifier = employerAccounts.TryGetValue(accountIdFromUrl, out var value)
                ? value : null;
        }

        if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
        {
            var requiredIdClaim = ClaimIdentifierConfiguration.Id;

            if (configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"] != null
                && configuration[$"{nameof(Infrastructure.Configuration.FeatureToggles)}:UseGovSignIn"]
                    .Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                requiredIdClaim = ClaimTypes.NameIdentifier;
            }

            if (!context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)))
            {
                return Task.FromResult(false);
            }

            var userClaim = context.User.Claims
                .First(c => c.Type.Equals(requiredIdClaim));

            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;

            var userId = userClaim.Value;

            var result = accountsService.GetUserAccounts(userId, email).Result;

            var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
            var associatedAccountsClaim = new Claim(ClaimIdentifierConfiguration.Account, accountsAsJson, JsonClaimValueTypes.Json);

            var updatedEmployerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(associatedAccountsClaim.Value);

            userClaim.Subject.AddClaim(associatedAccountsClaim);

            if (!updatedEmployerAccounts.TryGetValue(accountIdFromUrl, out var accountItem))
            {
                return Task.FromResult(false);
            }

            employerIdentifier = accountItem;
        }

        var hasAccess = CheckUserRoleForAccess(employerIdentifier, minimumAllowedRole);
        
        return Task.FromResult(hasAccess);
    }

    private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier, UserRole minimumAllowedRole)
    {
        var tryParse = Enum.TryParse<UserRole>(employerIdentifier.Role, true, out var userRole);

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