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
    IAccountClaimsService accountClaimsService,
    ILogger<EmployerAccountAuthorizationHandler> logger)
    : IEmployerAccountAuthorizationHandler
{
    public async Task<bool> IsEmployerAuthorized(AuthorizationHandlerContext context, UserRole minimumAllowedRole)
    {
        if (!httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue(RouteValueKeys.EncodedAccountId, out var routeValue))
        {
            return false;
        }

        var accountIdFromUrl = routeValue.ToString().ToUpper();
        var employerAccountClaim = context.User.FindFirst(c => c.Type.Equals(ClaimIdentifierConfiguration.Account));

        if (employerAccountClaim?.Value == null)
        {
            return false;
        }

        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = await accountClaimsService.GetAssociatedAccounts(forceRefresh: false);
        }
        catch (JsonSerializationException e)
        {
            logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        EmployerUserAccountItem employerIdentifier = null;

        if (employerAccounts != null)
        {
            employerIdentifier = employerAccounts.TryGetValue(accountIdFromUrl, out var value)
                ? value : null;
        }

        if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
        {
            if (!context.User.HasClaim(c => c.Type.Equals(ClaimTypes.NameIdentifier)))
            {
                return false;
            }

            var updatedEmployerAccounts = await accountClaimsService.GetAssociatedAccounts(forceRefresh: true);

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