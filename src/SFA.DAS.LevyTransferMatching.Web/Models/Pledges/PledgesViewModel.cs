using SFA.DAS.LevyTransferMatching.Domain.Types;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesViewModel : PledgesRequest
    {
        public IEnumerable<Pledge> Pledges { get; set; }
        public int PledgeCount => Pledges.Count();
        public bool RenderCreatePledgeButton { get; set; }
        
        public class Pledge
        {
            public string ReferenceNumber { get; set; }
            public int Amount { get; set; }
            public int RemainingAmount { get; set; }
            public int ApplicationCount { get; set; }
            public PledgeStatus Status { get; set; }
        }
    }
}
