﻿using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SectorRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }

        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        [AutoDecode("EncodedAccountId", Encoding.EncodingType.AccountId)]
        public long AccountId { get; set; }
    }
}