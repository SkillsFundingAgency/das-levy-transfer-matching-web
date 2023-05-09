using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.EmployerAccountsService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Authentication
{
    public class ManageAccountAuthorizationHandler : AuthorizationHandler<ManageAccountRequirement>
    {
        private readonly IEmployerAccountAuthorizationHandler _accountAuthorizationHandler;

        public ManageAccountAuthorizationHandler(IEmployerAccountAuthorizationHandler accountAuthorizationHandler)
        {
            _accountAuthorizationHandler = accountAuthorizationHandler;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAccountRequirement requirement)
        {
            var isAuthorized = await _accountAuthorizationHandler.IsEmployerAuthorized(context, UserRole.Transactor);

            if (isAuthorized)
            {
                context.Succeed(requirement);
                return;
            }
            
            context.Fail();
        }
    }
}