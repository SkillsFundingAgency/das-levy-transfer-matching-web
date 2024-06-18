using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Models;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[HideAccountNavigation(true)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    private readonly IStubAuthenticationService _stubAuthenticationService;

    public HomeController(ILogger<HomeController> logger, IConfiguration config, IStubAuthenticationService stubAuthenticationService)
    {
        _logger = logger;
        _config = config;
        _stubAuthenticationService = stubAuthenticationService;
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

    [Route("signout", Name = RouteNames.SignOut)]
    public async Task SignOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        _ = bool.TryParse(_config["StubAuth"], out var stubAuth);
        
        if (!stubAuth)
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/", Parameters = { {"id_token",idToken}}});    
        }
    }

    [Route("signoutcleanup")]
    public void SignOutCleanup()
    {
        Response.Cookies.Delete(CookieNames.Authentication);
    }
    
#if DEBUG
    [AllowAnonymous]
    [HttpGet]
    [Route("SignIn-Stub")]
    public IActionResult SigninStub()
    {
        return View("SigninStub", new List<string> { _config["StubId"], _config["StubEmail"] });
    }

    [AllowAnonymous()]
    [HttpPost]
    [Route("SignIn-Stub")]
    public async Task<IActionResult> SigninStubPost()
    {
        var claims = await _stubAuthenticationService.GetStubSignInClaims(new StubAuthUserDetails
        {
            Email = _config["StubEmail"],
            Id = _config["StubId"]
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
            new AuthenticationProperties());

        return RedirectToRoute("Signed-in-stub");
    }

    [Authorize()]
    [HttpGet]
    [Route("signed-in-stub", Name = "Signed-in-stub")]
    public IActionResult SignedInStub()
    {
        return View();
    }
#endif
}