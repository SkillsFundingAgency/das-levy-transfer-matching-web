using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class MoreDetailsPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        public string Details { get; set; }
    }
}
