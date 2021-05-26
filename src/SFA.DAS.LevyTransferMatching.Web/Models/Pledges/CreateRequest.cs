using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
    }
}
