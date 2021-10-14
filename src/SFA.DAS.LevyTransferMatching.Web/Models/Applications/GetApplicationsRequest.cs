using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class GetApplicationsRequest
    {
        public string EncodedAccountId { get; set; }
        [AutoDecode(nameof(EncodedAccountId), Encoding.EncodingType.AccountId)]
        public long AccountId { get; set; }
    }
}
