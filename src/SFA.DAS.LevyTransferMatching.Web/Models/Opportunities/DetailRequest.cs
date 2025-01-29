using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

public class DetailRequest
{
    public string EncodedPledgeId { get; set; }

    [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
    public int PledgeId { get; set; }
    public int? Page { get; set; }
    public string SortBy { get; set; }
    public string CommaSeparatedSectors { get; set; }

}