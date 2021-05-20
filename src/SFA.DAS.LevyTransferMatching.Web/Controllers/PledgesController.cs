﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    [Route("accounts/{EncodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        public IActionResult Index(string encodedAccountId)
        {
            var viewModel = new IndexViewModel()
            {
                EncodedAccountId = encodedAccountId,
            };

            return View(viewModel);
        }

        [Route("create")]
        public IActionResult Create(string encodedAccountId)
        {
            var viewModel = new CreateViewModel()
            {
                EncodedAccountId = encodedAccountId,
            };

            return View(viewModel);
        }

        public IActionResult PledgeAmount()
        {
            return View(new PledgeAmountViewModel());
        }

        [HttpPost]
        public IActionResult PledgeAmount(PledgeAmountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("Index");
        }
    }
}