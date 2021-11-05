using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationResponse
    {
        public IEnumerable<string> Sector { get; set; }
        public string TypeOfJobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public int MaxFunding { get; set; }
        public DateTime StartBy { get; set; }
        public bool HasTrainingProvider { get; set; }
        public string AboutOpportunity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
        public string EmployerAccountName { get; set; }
        public List<string> PledgeSectors { get; set; }
        public List<string> PledgeLevels { get; set; }
        public List<string> PledgeJobRoles { get; set; }
        public List<string> PledgeLocations { get; set; }
        public int PledgeRemainingAmount { get; set; }
        public int Amount { get; set; }
        public ApplicationStatus Status { get; set; }
        public IEnumerable<ReferenceDataItem> AllJobRoles { get; set; }
        public IEnumerable<ReferenceDataItem> AllSectors { get; set; }
        public IEnumerable<ReferenceDataItem> AllLevels { get; set; }
        public IEnumerable<string> Locations { get; set; }
        public string AdditionalLocation { get; set; }
        public string SpecificLocation { get; set; }
    }
}
