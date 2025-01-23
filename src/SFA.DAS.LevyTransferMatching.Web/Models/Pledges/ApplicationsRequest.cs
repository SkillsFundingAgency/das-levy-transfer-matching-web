using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsRequest
    {
        public string EncodedAccountId { get; set; }
        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public int AccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public SortColumn? SortColumn { get; set; }
        public SortOrder? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public const int PageSize = 15;
    }
}
