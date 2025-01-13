using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication;

public class ViewAccountRequirement : IAuthorizationRequirement;

public class ViewAccountAuthorizationHandler(
    IEmployerAccountAuthorizationHandler accountAuthorizationHandler)
    : AuthorizationHandler<ViewAccountRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewAccountRequirement requirement)
    {
        var isAuthorized = await accountAuthorizationHandler.IsEmployerAuthorized(context, UserRole.Viewer);

        if (isAuthorized)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}