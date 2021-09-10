using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationSelectPostRequest : LocationSelectRequest
    {
        public IDictionary<int, string> SelectedValidLocations { get; set; }
    }
}