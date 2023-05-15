using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationDetailsPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        [AutoDecode("EncodedAccountId", Encoding.EncodingType.AccountId)]
        public int AccountId { get; set; }

        public string NumberOfApprentices { get; set; }
        public int? ParsedNumberOfApprentices => int.TryParse(NumberOfApprentices, out var result) ? result : default(int?);
        public DateTime? StartDate
        {
            get 
            {
                if (DateTime.TryParse($"{Year}-{Month}-01", out DateTime date))
                {
                    return date;
                }

                return null;
            }
        }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool? HasTrainingProvider { get; set; }
        public string SelectedStandardId { get; set; }
        public string SelectedStandardTitle { get; set; }
        public bool ExceedsAvailableFunding { get; set; }
    }
}