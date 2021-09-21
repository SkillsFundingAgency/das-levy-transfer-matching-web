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
            EmailAddresses = new List<string>();
        }

        public Guid Key { get; set; }

        public string Details { get; set; }
        public string StandardId { get; set; }
        public string JobRole { get; set; }
        public int? NumberOfApprentices { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? HasTrainingProvider { get; set; }
        public List<string> Sectors { get; set; }

        public string FirstName { get; set; }
        public int Amount { get; set; }
        public string LastName { get; set; }
        public List<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
    }
}