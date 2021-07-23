using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetMyPledgesResponse
    {
        public IEnumerable<MyPledge> Pledges { get; set; }

        public class MyPledge
        {
            public int Id { get; set; }
            public int Amount { get; set; }
            public int NumberOfApplications { get; set; }
        }
    }
}
