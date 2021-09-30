using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class ApplicationViewModel : ApplicationRequest
    {
        public IEnumerable<string> Locations { get; set; }
        public bool IsNamePublic { get; set; }
        public string EmployerAccountName { get; set; }
        public ApplicationStatus Status { get; set; }
        public string JobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public string Amount { get; set; }
        public DateTime StartBy { get; set; }
        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
        public string EncodedOpportunityId { get; set; }
    }
}