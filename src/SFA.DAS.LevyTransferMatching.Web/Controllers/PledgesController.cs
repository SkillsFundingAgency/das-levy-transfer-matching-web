using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    // [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    public class PledgesController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PledgesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("accounts/{" + ControllerConstants.AccountHashedIdRouteKeyName + "}/pledges")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("accounts/{" + ControllerConstants.AccountHashedIdRouteKeyName + "}/pledges/create")]
        public IActionResult Create()
        {
            return View();
        }
    }
}