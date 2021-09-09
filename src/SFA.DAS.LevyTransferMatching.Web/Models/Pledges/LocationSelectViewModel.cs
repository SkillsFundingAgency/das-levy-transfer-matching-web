using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationSelectViewModel : LocationSelectPostRequest
    {
        public IDictionary<int, IEnumerable<string>> MultipleValidLocations { get; set; }
    }
}