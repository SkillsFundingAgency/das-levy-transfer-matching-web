using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsForDownloadResponse
    {
        public class Application
        {
            public int PledgeId { get; set; }
            public long ApplicationId { get; set; }
            public DateTime DateApplied { get; set; }
            public ApplicationStatus Status { get; set; }
            public string EmployerAccountName { get; set; }
            public IEnumerable<string> Locations { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public int NumberOfApprentices { get; set; }
            public DateTime StartBy { get; set; }
            public bool HasTrainingProvider { get; set; }
            public string AboutOpportunity { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public IEnumerable<string> EmailAddresses { get; set; }
            public string BusinessWebsite { get; set; }
            public bool MatchSector { get; set; }
            public bool MatchJobRole { get; set; }
            public bool MatchLevel { get; set; }
            public bool MatchLocation { get; set; }
            public int MatchPercentage { get; set; }
            public string JobRole { get; set; }
            public int Amount { get; set; }
            public int PledgeRemainingAmount { get; set; }
            public int MaxFunding { get; set; }
            public int EstimatedDurationMonths { get; set; }
        }

        public IEnumerable<Application> Applications { get; set; }
    }
}
