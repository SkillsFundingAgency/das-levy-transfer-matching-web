using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class MyPledgesViewModel : MyPledgesRequest
    {
        public IEnumerable<MyPledge> Pledges { get; set; }
        public int PledgeCount => Pledges.Count();

        public class MyPledge
        {
            public string ReferenceNumber { get; set; }
            public int Amount { get; set; }
            public int RemainingAmount { get; set; }
            public int ApplicationCount { get; set; }
        }
    }
}
