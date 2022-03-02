using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsViewModel
    {
        public string EncodedAccountId { get; set; }
        public string TaxYear => DateTime.UtcNow.ToTaxYearDescription();
        public string EncodedPledgeId { get; set; }
        public IEnumerable<Application> Applications { get; set; }
        public bool DisplayRejectedBanner { get; set; }
        public string RejectedEmployerName { get; set; }
        public bool UserCanClosePledge { get; set; }
        public bool RenderCreatePledgeButton { get; set; }

        public class Application
        {
            public string DasAccountName { get; set; }
            public string EncodedApplicationId { get; set; }
            public int Amount { get; set; }
            public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
            public int Duration { get; set; }
            public ApplicationStatus Status { get; set; }
            public bool IsSectorMatch { get; set; }
            public bool IsJobRoleMatch { get; set; }
            public bool IsLevelMatch { get; set; }
            public bool IsLocationMatch { get; set; }
            public DateTime CreatedOn { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string BusinessWebsite { get; set; }
            public DateTime StartBy { get; set; }
            public IEnumerable<string> EmailAddresses { get; set; }
            public string JobRole { get; set; }
            public int PledgeRemainingAmount { get; set; }
            public int MaxFunding { get; set; }
            public string Details { get; set; }
        }
    }
}
