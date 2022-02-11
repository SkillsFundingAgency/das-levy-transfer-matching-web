using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexRequest : ApplyRequest
    {
        public List<string> Sectors { get; set; }
    }
}