using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class OpportunityDto
    {
        public int Amount { get; set; }
        public string DasAccountName { get; set; }
        public string EncodedPledgeId { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
    }
}
