using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesViewModel : PledgesRequest
    {
        public IEnumerable<Pledge> Pledges { get; set; }
        public PagingData Paging { get; set; }
        public bool RenderCreatePledgeButton { get; set; }
        public bool HasMinimumTransferFunds { get; set; }

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
