using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("search-funding")]
    [HideAccountNavigation(true)]
    public class SearchFunding : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
