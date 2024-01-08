using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications;

public class ApplicationRequest
{
    public string EncodedAccountId { get; set; }
    [AutoDecode(nameof(EncodedAccountId), Encoding.EncodingType.AccountId)]
    public long AccountId { get; set; }

    public string EncodedApplicationId { get; set; }

    [AutoDecode(nameof(EncodedApplicationId), Encoding.EncodingType.PledgeApplicationId)]
    public int ApplicationId { get; set; }
        
    public string EncodedSenderPublicAccountId { get; set; }
}