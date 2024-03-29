﻿using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using System.Collections.Generic;
using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexViewModel
    {
        public List<Opportunity> Opportunities { get; set; }
        public List<ReferenceDataItem> Sectors { get; set; }
        public bool isSectorFilterApplied { get; set; }

        public class Opportunity
        {
            public int Amount { get; set; }
            public string EmployerName { get; set; }
            public string ReferenceNumber { get; set; }
            public string Locations { get; set; }
            public string Sectors { get; set; }
            public string JobRoles { get; set; }
            public string Levels { get; set; }
            public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
        }
    }
}
