using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    public class PledgesController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PledgesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}