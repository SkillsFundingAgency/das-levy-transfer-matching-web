using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications;

public class GetApplicationsRequest
{
    public string EncodedAccountId { get; set; }
    [AutoDecode(nameof(EncodedAccountId), Encoding.EncodingType.AccountId)]
    public long AccountId { get; set; }
    public int? Page { get; set; } = 1;
    public const int PageSize = 15;
}