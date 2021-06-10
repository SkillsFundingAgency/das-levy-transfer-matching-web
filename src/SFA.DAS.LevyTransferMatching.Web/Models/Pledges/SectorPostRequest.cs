using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorPostRequest : PledgesRequest
    {
        public List<string> Sectors { get; set; }
    }
}