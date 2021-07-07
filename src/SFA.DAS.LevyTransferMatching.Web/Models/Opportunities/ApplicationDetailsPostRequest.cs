using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationDetailsPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public string JobRole { get; set; }
        public int? NumberOfApprentices { get; set; }
        public DateTime? StartDate
        {
            get 
            {
                DateTime x;
                if (DateTime.TryParse($"{Year}-{Month}-01", out x))
                    return x;

                return null;
            }
        }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool? HasTrainingProvider { get; set; }
    }
}
