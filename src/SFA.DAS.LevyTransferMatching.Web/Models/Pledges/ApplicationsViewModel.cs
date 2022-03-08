using System;
using System.Collections.Generic;
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
    }
}
