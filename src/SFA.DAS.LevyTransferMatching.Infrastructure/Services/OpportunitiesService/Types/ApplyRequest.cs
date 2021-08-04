using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class ApplyRequest
    {
        public string EncodedAccountId { get; set; }

        public string Details { get; set; }

        public string StandardId { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartDate { get; set; }
        public bool HasTrainingProvider { get; set; }

        public IEnumerable<string> Sectors { get; set; }
        public string Postcode { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
    }

    public class ApplyResponse
    {
        public int ApplicationId { get; set; }
    }
}