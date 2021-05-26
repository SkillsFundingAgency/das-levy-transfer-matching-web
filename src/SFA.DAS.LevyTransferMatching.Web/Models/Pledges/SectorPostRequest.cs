using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorPostRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public string Amount { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}