using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsAccountNamesResponse
    {
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
        }
    }
}
