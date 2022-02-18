using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationViewModel : ApplicationPostRequest
    {
        public string DisplaySectors { get; set; }
        public string DasAccountName { get; set; }
        public string Details { get; set; }
        public int NumberOfApprentices { get; set; }
        public int Amount { get; set; }
        public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
        public bool HasTrainingProvider { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessWebsite { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Duration { get; set; }
        public ApplicationStatus Status { get; set; }
        public string JobRole { get; set; }
        public int Level { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public DateTime StartBy { get; set; }
        public string AboutOpportunity { get; set; }
        public IEnumerable<ReferenceDataItem> AllSectors { get; set; }
        public List<string> PledgeSectors { get; set; }
        public List<string> PledgeJobRoles { get; set; }
        public List<string> PledgeLevels { get; set; }
        public List<string> PledgeLocations { get; set; }
        public string Locations { get; set; }
        public bool IsSectorMatch { get; set; }
        public bool IsJobRoleMatch { get; set; }
        public bool IsLevelMatch { get; set; }
        public bool IsLocationMatch { get; set; }
        public int MatchPercentage { get; set; }
        public int PledgeRemainingAmount { get; set; }
        public int MaxFunding { get; set; }
        public AffordabilityViewModel Affordability { get; set; }

        public bool AllowApproval { get; set; }
        public bool AllowTransferRequestAutoApproval { get; set; }
        public string PercentageMatchCssClass => this.GetMatchPercentageCss();

        public class AffordabilityViewModel
        {
            public string RemainingFunds { get; set; }
            public string EstimatedCostThisYear { get; set; }
            public string RemainingFundsIfApproved { get; set; }
            public string EstimatedCostOverDuration { get; set; }
            public string YearDescription { get; set; }
        }
    }
}
