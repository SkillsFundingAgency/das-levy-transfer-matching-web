using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsViewModel
    {
        public string EncodedAccountId { get; set; }
        public string TaxYear => DateTime.UtcNow.ToTaxYearDescription();
        public string EncodedPledgeId { get; set; }
        public IEnumerable<ApplicationViewModel> Applications { get; set; }
    }
}
