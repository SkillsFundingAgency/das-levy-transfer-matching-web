using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("search-funding")]
    public class SearchFundingController : Controller
    {
        public IActionResult Index()
        {
            Opportunity[] opportunities = new Opportunity[]
            {
                new Opportunity() { EmployerName = "Company One", ReferenceNumber = "COMP1" },
                new Opportunity() { EmployerName = "Company Two", ReferenceNumber = "COMP2" },
                new Opportunity() { EmployerName = null, ReferenceNumber = "ABC123" },
                new Opportunity() { EmployerName = "Company Three", ReferenceNumber = "COMP3" }
            };

            return View(opportunities);
        }
    }
}
