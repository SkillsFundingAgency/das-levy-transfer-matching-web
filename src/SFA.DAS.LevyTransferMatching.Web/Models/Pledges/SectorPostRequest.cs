using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorPostRequest : BasePledgesRequest
    {
        public List<string> Sectors { get; set; }
    }
}