using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication;

public class ManageAccountRequirement : IAuthorizationRequirement;

public class ManageAccountAuthorizationHandler(
    IEmployerAccountAuthorizationHandler accountAuthorizationHandler)
    : AuthorizationHandler<ManageAccountRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAccountRequirement requirement)
    {
        var isAuthorized = await accountAuthorizationHandler.IsEmployerAuthorized(context, UserRole.Transactor);

        if (isAuthorized)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}