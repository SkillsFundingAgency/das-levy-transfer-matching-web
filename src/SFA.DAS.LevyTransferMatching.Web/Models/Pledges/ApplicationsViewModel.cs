using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsViewModel : ApplicationsPostRequest
    {
        public string TaxYear => DateTime.UtcNow.ToTaxYearDescription();
        public IEnumerable<ApplicationViewModel> Applications { get; set; }
        public bool UserCanClosePledge { get; set; }
        public bool DisplayRejectedBanner { get; set; }
        public string RejectedEmployerName { get; set; }
        public bool RenderCreatePledgeButton { get; set; }
        public bool RenderRejectButton { get; set; }
        public int ApplicationsPendingApproval { get => Applications.Count(x => x.Status == ApplicationStatus.Pending); }
        public bool RenderApplicationsList { get => Applications != null && Applications.Any(); }
    }
}
