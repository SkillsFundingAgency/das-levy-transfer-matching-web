using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetOpportunityApplyResponse
    {
        public IEnumerable<Account> Accounts { get; set; }

        public class Account
        {
            public string EncodedAccountId { get; set; }
            public string Name { get; set; }
        }
    }
}