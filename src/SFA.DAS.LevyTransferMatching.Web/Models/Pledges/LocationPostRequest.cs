using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationPostRequest : BasePledgesRequest
    {
        public List<string> Locations { get; set; }
    }
}
