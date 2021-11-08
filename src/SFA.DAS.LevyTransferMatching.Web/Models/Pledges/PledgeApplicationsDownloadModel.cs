﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
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
        public IEnumerable<string> Locations { get; set; }
        public string AdditionalLocation { get; set; }
        public string SpecificLocation { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public string TypeOfJobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartBy { get; set; }
        public bool HasTrainingProvider { get; set; }
        public string AboutOpportunity { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
        public IEnumerable<string> PledgeLocations { get; set; }
        public IEnumerable<ReferenceDataItem> AllSectors { get; set; }
        public IEnumerable<ReferenceDataItem> AllLevels { get; set; }
        public IEnumerable<ReferenceDataItem> AllJobRoles { get; set; }
        public string ContactName => $"{FirstName} {LastName}";
        public int Duration { get; set; }
        public string FormattedEmailAddress { get; set; }
        public string FormattedSectors { get; set; }
        public string FormattedLocations { get; set; }
    }
}
