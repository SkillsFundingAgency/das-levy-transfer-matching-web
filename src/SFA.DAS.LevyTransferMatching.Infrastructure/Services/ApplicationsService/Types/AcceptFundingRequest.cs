using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types
{
    public class AcceptFundingRequest
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int ApplicationId { get; set; }
        public long AccountId { get; set; }
    }
}
