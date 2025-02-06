using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

public class StandardsListItemDto
{
    public string StandardUId { get; set; }
    public int LarsCode { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public List<ApprenticeshipFundingDto> ApprenticeshipFunding { get; set; }

}