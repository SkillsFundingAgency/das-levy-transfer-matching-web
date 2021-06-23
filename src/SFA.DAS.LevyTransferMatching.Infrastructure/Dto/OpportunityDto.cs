﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class OpportunityDto
    {
        public string EncodedPledgeId { get; set; }
        public string DasAccountName { get; set; }
        public IEnumerable<string> JobRoles { get; set; }
        public IEnumerable<string> Levels { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
