using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Account;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Route("accounts/{encodedAccountId}")]
    
    public class AccountController : LoggedInController
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

            return View(viewmodel);
        }
    }
}
