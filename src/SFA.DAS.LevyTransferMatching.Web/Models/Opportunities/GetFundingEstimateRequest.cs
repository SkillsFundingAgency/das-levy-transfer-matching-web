using System;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class GetFundingEstimateRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public string SelectedStandardId { get; set; }
        public DateTime StartDate { get; set; }
        public int NumberOfApprentices { get; set; }
    }
}
