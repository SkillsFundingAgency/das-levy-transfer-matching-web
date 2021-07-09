using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexViewModel
    {
        public List<Opportunity> Opportunities { get; set; }
        public string TaxYear => DateTime.UtcNow.ToTaxYearDescription();
    }
}
