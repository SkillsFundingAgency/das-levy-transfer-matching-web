﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
    }
}