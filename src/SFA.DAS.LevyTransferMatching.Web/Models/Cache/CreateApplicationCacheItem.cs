using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreateApplicationCacheItem
    {
        public CreateApplicationCacheItem()
        {
            Key = Guid.NewGuid();
        }

        public CreateApplicationCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public string Details { get; set; }
        public string StandardId { get; set; }
        public string JobRole { get; set; }
        public int? NumberOfApprentices { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? HasTrainingProvider { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public List<string> AdditionalEmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
    }
}