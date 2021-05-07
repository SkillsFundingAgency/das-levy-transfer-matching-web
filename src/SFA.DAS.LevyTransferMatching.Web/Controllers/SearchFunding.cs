using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    public class SearchFunding : Controller
    {
        [Route("search-funding-opportunities")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
