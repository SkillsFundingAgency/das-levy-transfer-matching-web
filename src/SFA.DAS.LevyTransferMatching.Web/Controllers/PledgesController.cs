using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    [Route("accounts/{EncodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        private readonly IPledgeOrchestrator _orchestrator;

        public PledgesController(IPledgeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        public IActionResult Index(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetIndexViewModel(encodedAccountId);
            return View(viewModel);
        }

        [Route("create")]
        public IActionResult Create(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetCreateViewModel(encodedAccountId);
            return View(viewModel);
        }

        [Route("create/amount")]
        public IActionResult Amount(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetAmountViewModel(encodedAccountId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/amount")]
        public IActionResult Amount(AmountPostModel viewModel)
        {
            return RedirectToAction("Index", new { encodedAccountId = viewModel.EncodedAccountId });
        }
    }
}