using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication;

public class ViewAccountAuthorizationHandler : AuthorizationHandler<ViewAccountRequirement>
{
    private readonly IEmployerAccountAuthorizationHandler _accountAuthorizationHandler;
        
    public ViewAccountAuthorizationHandler(IEmployerAccountAuthorizationHandler accountAuthorizationHandler)
    {
        _accountAuthorizationHandler = accountAuthorizationHandler;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewAccountRequirement requirement)
    {
        var isAuthorized = await _accountAuthorizationHandler.IsEmployerAuthorized(context, UserRole.Viewer);

        if (isAuthorized)
        {
            context.Succeed(requirement);
            return;
        }
            
        context.Fail();
    }

}