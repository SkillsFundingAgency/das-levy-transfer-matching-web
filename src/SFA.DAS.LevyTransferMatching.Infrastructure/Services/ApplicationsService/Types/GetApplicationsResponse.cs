using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types
{
    public class GetApplicationsResponse
    {
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
            public int PledgeId { get; set; }
            public string Details { get; set; }
            public int NumberOfApprentices { get; set; }
            public int Amount { get; set; }
            public int TotalAmount { get; set; }
            public DateTime CreatedOn { get; set; }
            public ApplicationStatus Status { get; set; }
            public bool IsNamePublic { get; set; }
        }
    }
}
