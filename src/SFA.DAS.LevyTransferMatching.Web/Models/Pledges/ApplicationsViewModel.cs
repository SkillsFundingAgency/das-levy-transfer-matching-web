using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsViewModel : ApplicationsPostRequest
    {
    
        public IEnumerable<Application> Applications { get; set; }
        public PagingData Paging { get; set; }
        public bool UserCanClosePledge { get; set; }
        public bool DisplayRejectedBanner { get; set; }
        public string RejectedEmployerName { get; set; }
        public bool RenderCreatePledgeButton { get; set; }
        public bool RenderRejectButton { get; set; }
        public string PledgeTotalAmount { get; set; }
        public string PledgeRemainingAmount { get; set; }
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }
        public int ApplicationsPendingApproval { get => Applications.Count(x => x.Status == ApplicationStatus.Pending); }
        public bool RenderApplicationsList { get => Applications != null && Applications.Any(); }

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
            public int? RemainingDaysForDelayedApproval { get; set; }
            public int? RemainingDaysForAutoRejection { get; set; }
        }
    }
}
