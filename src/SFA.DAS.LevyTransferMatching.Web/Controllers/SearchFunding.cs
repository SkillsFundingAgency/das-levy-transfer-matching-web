using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("search-funding")]
    public class SearchFunding : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
