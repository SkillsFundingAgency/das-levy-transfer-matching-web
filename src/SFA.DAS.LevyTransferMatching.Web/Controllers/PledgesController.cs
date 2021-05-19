using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    // [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    [Route("accounts/{EncodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        public PledgesController()
        {
        }

        public IActionResult Index()
        {
            var viewModel = this.CreateViewModel<IndexViewModel>(this.RouteData);

            return View(viewModel);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var viewModel = this.CreateViewModel<CreateViewModel>(this.RouteData);

            return View(viewModel);
        }

        private TPledgesViewModel CreateViewModel<TPledgesViewModel>(RouteData routeData) where TPledgesViewModel : PledgesViewModel, new()
        {
            string encodedAccountId = (string)routeData.Values[RouteValueKeys.EncodedAccountId];

            TPledgesViewModel pledgesViewModel = new TPledgesViewModel()
            {
                EncodedAccountId = encodedAccountId,
            };

            return pledgesViewModel;
        }
    }
}