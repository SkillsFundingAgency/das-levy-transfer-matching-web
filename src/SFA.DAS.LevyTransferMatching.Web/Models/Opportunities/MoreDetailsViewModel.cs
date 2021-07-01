using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class MoreDetailsViewModel : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
        public string Details { get; set; }
    }
}
