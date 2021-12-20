using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class ClosePledgeRequest
    {
        public int PledgeId { get; set; }
        public int status { get; set; }
    }
}
