using System;
using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorPostRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public Sector? Sectors { get; set; }
    }
}