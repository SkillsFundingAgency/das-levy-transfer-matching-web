using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsResponse
    {
        public StandardsListItemDto Standard { get; set; }
        public IEnumerable<Application> Applications { get; set; }

        public class Application
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
            public int PledgeId { get; set; }
            public string Details { get; set; }
            public string StandardId { get; set; }
            public int NumberOfApprentices { get; set; }
            public DateTime StartDate { get; set; }
            public int Amount { get; set; }
            public bool HasTrainingProvider { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public string Postcode { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string BusinessWebsite { get; set; }
            public IEnumerable<string> EmailAddresses { get; set; }
            public DateTime CreatedOn { get; set; }
        }
    }
}
