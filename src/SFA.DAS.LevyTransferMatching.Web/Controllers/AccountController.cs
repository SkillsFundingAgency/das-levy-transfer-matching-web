using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Account;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Route("accounts/{encodedAccountId}")]
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IActionResult Index(string encodedAccountId)
        {
            var viewmodel = new IndexViewModel
            {
                UserId = _authenticationService.UserId,
                DisplayName = _authenticationService.UserDisplayName
            };

            ViewBag.HideNav = false;

            return View(viewmodel);
        }
    }
}
