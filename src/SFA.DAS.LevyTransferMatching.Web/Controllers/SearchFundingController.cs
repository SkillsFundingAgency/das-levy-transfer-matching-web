using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("search-funding")]
    public class SearchFundingController : Controller
    {
        private readonly ISearchFundingOrchestrator _searchFundingOrchestrator;

        public SearchFundingController(ISearchFundingOrchestrator searchFundingOrchestrator)
        {
            _searchFundingOrchestrator = searchFundingOrchestrator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _searchFundingOrchestrator.GetSearchFundingViewModel();
            return View(viewModel);
        }
    }
}
