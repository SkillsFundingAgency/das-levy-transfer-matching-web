using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationDetailsPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        public List<ReferenceDataItem> JobRoles { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartDate { get; set; }
        public bool HasTrainingProvider { get; set; }
    }
}
