﻿using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LevyTransferMatching.Web.Models
{
    public class PledgeAmountViewModel
    {
        public string PledgeAmount { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}
