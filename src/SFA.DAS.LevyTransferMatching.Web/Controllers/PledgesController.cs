using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    // [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    public class PledgesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PledgesOrchestrator _pledgesOrchestrator;

        public PledgesController(
            ILogger<HomeController> logger,
            PledgesOrchestrator pledgesOrchestrator)
        {
            _logger = logger;
            _pledgesOrchestrator = pledgesOrchestrator;
        }

        [Route("accounts/{" + RouteValueKeys.EncodedAccountId + "}/pledges")]
        public IActionResult Index()
        {
            var viewModel = _pledgesOrchestrator.Index(this.RouteData);

            return View(viewModel);
        }

        [Route("accounts/{" + RouteValueKeys.EncodedAccountId + "}/pledges/create")]
        public IActionResult Create()
        {
            var viewModel = _pledgesOrchestrator.Create(this.RouteData);

            return View(viewModel);
        }
    }
}