﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    public class FindBusinessController : Controller
    {
        [Route("find-a-business")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
