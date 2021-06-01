using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AmountPostRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public string Amount { get; set; }
        public string RemainingTransferAllowance { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}