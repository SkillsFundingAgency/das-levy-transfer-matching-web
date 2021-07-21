using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreateApplicationCacheItem
    {
        private IEnumerable<string> _additionalEmailAddresses;

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
        public IEnumerable<string> EmailAddresses
        {
            get
            {
                if (_additionalEmailAddresses == null)
                {
                    _additionalEmailAddresses = Enumerable.Range(0, 5).Select(x => (string)null).ToArray();
                }

                return _additionalEmailAddresses;
            }

            set
            {
                _additionalEmailAddresses = value;
            }
        }
        public string BusinessWebsite { get; set; }
    }
}