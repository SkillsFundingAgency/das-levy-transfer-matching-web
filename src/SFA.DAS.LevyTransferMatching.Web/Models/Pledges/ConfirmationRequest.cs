using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ConfirmationRequest
    {
        public string EncodedPledgeId { get; set; }
        public string EncodedAccountId { get; set; }
    }
}
