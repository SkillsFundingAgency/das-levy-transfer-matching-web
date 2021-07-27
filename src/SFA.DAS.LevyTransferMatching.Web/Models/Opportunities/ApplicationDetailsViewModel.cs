using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationDetailsViewModel : ApplyRequest
    {
        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
        public string EncodedPledgeId { get; set; }
        public string JobRole { get; set; }
        public int? NumberOfApprentices { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? StartDate
        {
            get => Year.HasValue && Month.HasValue ? new DateTime(Year.Value, Month.Value, 1) : new DateTime?();
        }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public bool? HasTrainingProvider { get; set; }
        public SelectStandardViewModel SelectStandardViewModel { get; set; }
        public int SelectedStandardId { get; set; }
    }
}