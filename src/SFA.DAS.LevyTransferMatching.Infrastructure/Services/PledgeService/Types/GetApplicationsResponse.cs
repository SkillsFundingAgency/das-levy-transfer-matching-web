using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsResponse : PagedResponse<GetApplicationsResponse.Application>
    {
        public PledgeStatus PledgeStatus { get; set; }
        public int PledgeRemainingAmount { get; set; }
        public int PledgeTotalAmount { get; set; }
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
            public int PledgeId { get; set; }
            public int StandardDuration { get; set; }
            public string Details { get; set; }
            public DateTime StartDate { get; set; }
            public int Amount { get; set; }
            public int TotalAmount { get; set; }
            public int CurrentFinancialYearAmount { get; set; }
            public bool HasTrainingProvider { get; set; }
            public int NumberOfApprentices { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public string Postcode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string BusinessWebsite { get; set; }
            public IEnumerable<string> EmailAddresses { get; set; }

            public DateTime CreatedOn { get; set; }
            public ApplicationStatus Status { get; set; }
            public bool IsNamePublic { get; set; }
            public bool IsLocationMatch { get; set; }
            public bool IsSectorMatch { get; set; }
            public bool IsJobRoleMatch { get; set; }
            public bool IsLevelMatch { get; set; }
            public int PledgeRemainingAmount { get; set; }
            public int MaxFunding { get; set; }
            public string JobRole { get; set; }
            public string EmployerAccountName { get; set; }
            public int Level { get; set; }
            public string AdditionalLocations { get; set; }
            public string SpecificLocation { get; set; }
            public IEnumerable<GetApplyResponse.PledgeLocation> PledgeLocations { get; set; }
            public IEnumerable<ApplicationLocation> Locations { get; set; }
        }

        public class ApplicationLocation
        {
            public int Id { get; set; }
            public int PledgeLocationId { get; set; }
        }
    }
}
