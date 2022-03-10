using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgeApplicationsDownloadModel
    {
        public IEnumerable<PledgeApplicationDownloadModel> Applications { get; set; }
    }

    public class PledgeApplicationDownloadModel 
    {
        public string EncodedPledgeId { get; set; }
        public int PledgeId { get; set; }
        public long ApplicationId { get; set; }
        public string EncodedApplicationId { get; set; }
        public DateTime DateApplied { get; set; }
        public ApplicationStatus Status { get; set; }
        public string EmployerAccountName { get; set; }
        public IEnumerable<GetApplicationsResponse.ApplicationLocation> Locations { get; set; }
        public IEnumerable<GetApplyResponse.PledgeLocation> PledgeLocations { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public string TypeOfJobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartBy { get; set; }
        public bool HasTrainingProvider { get; set; }
        public string AboutOpportunity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessWebsite { get; set; }
        public string ContactName => $"{FirstName} {LastName}";
        public int Duration { get; set; }
        public string FormattedEmailAddress { get; set; }
        public string FormattedSectors { get; set; }
        public string FormattedLocations { get; set; }
        public bool MatchSector { get; set; }
        public bool MatchJobRole { get; set; }
        public bool MatchLevel { get; set; }
        public bool MatchLocation { get; set; }
        public int MatchPercentage { get; set; }
        public int EstimatedCostThisYear { get; set; }
        public int TotalEstimatedCost { get; set; }
        public string AdditionalLocations { get; set; }
        public string SpecificLocation { get; set; }
        public IEnumerable<dynamic> DynamicLocations { get; set; }
    }
}
