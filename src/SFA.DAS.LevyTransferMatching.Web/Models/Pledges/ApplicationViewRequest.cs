using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationViewRequest : ApplicationsRequest
    {
        [AutoDecode(nameof(EncodedApplicationId), EncodingType.PledgeApplicationId)]
        public int ApplicationId { get; set; }
        
        public string EncodedApplicationId { get; set; }
    }
}
