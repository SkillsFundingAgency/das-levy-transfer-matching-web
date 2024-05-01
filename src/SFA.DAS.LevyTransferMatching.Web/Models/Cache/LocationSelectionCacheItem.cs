using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class LocationSelectionCacheItem
    {
        public LocationSelectionCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public IDictionary<int, IEnumerable<string>> MultipleValidLocations { get; set; }
    }
}