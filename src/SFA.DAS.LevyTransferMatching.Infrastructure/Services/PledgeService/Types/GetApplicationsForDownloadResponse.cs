using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsForDownloadResponse
    {
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int PledgeId { get; set; }
            public long ApplicationId { get; set; }
            public DateTime DateApplied { get; set; }
            public string Status { get; set; }
            public string EmployerAccountName { get; set; }
            public IEnumerable<string> Locations { get; set; }
            public string AdditionalLocation { get; set; }
            public string SpecificLocation { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public string TypeOfJobRole { get; set; }
            public int Level { get; set; }
            //public IEnumerable<string> JobRoles { get; set; }
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
            public List<ReferenceDataItem> AllSectors { get; set; }
            public List<ReferenceDataItem> AllLevels { get; set; }
            public List<ReferenceDataItem> AllJobRoles { get; set; }
        }
    }
}
