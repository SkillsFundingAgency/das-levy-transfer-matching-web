﻿using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectApplicationsPostRequest : BasePledgesRequest
    {
        public string EncodedPledgeId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        public List<string> ApplicationsToReject { get; set; }

        public bool? RejectConfirm { get; set; }
    }
}