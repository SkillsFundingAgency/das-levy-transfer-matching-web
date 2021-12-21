﻿using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CloseViewModel : BasePledgesRequest
    {
        public bool? HasConfirmed { get; set; }

        public string EncodedPledgeId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        public bool UserCanClosePledge { get; set; }

        public bool PledgeClosed { get; set; }
    }
}