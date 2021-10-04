using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class AcceptFundingPostRequest
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int ApplicationId { get; set; }
        public long AccountId { get; set; }
    }
}
