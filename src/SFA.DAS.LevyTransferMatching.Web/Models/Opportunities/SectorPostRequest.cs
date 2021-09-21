using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SectorPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }

        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        [AutoDecode("EncodedAccountId", Encoding.EncodingType.AccountId)]
        public long AccountId { get; set; }

        public List<string> Sectors { get; set; }

        public bool HasPledgeLocations { get; set; }
        public List<int> Locations { get; set; }
        public bool AdditionalLocation { get; set; }
        public string AdditionalLocationText { get; set; }
        public string SpecificLocation { get; set; }
    }
}