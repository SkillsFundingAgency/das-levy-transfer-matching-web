using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Models;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("signout", Name= "signout")]
        public IActionResult SignOut()
        {
            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = "",
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Route("signoutcleanup")]
        public void SignOutCleanup()
        {
            Response.Cookies.Delete("SFA.DAS.LevyTransferMatching.Web.Auth");
        }

        [Authorize]
        [Route("/secure")]
        public IActionResult Secure()
        {
            var userName =
                HttpContext.User.Claims.FirstOrDefault(x => x.Type == $"{ClaimIdentifierConfiguration.ClaimsBaseUrl}{ClaimIdentifierConfiguration.DisplayName}");

            var viewmodel = new SecureViewModel{ DisplayName = userName?.Value };
            
            return View(viewmodel);
        }


        //[Authorize]
        [Route("/accounts/{accountId}")]
        public IActionResult Accounts(string accountId)
        
        {
            var userName =
                HttpContext.User.Claims.FirstOrDefault(x => x.Type == $"{ClaimIdentifierConfiguration.ClaimsBaseUrl}{ClaimIdentifierConfiguration.DisplayName}");

            var viewmodel = new SecureViewModel { DisplayName = userName?.Value };

            ViewBag.HideNav = false;

            return View(viewmodel);
        }

    }
}
