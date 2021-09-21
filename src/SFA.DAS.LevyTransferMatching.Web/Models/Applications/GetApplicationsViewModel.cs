using System;
using System.Collections.Generic;
using System.Globalization;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class GetApplicationsViewModel
    {
        public IEnumerable<ApplicationViewModel> Applications { get; set; }

        public class ApplicationViewModel
        {
            public string DasAccountName { get; set; }
            public int NumberOfApprentices { get; set; }
            public int Amount { get; set; }
            public DateTime CreatedOn { get; set; }
            public int Duration { get; set; }
            public ApplicationStatus Status { get; set; }
            public string EncodedApplicationId { get; set; }
        }
    }
}