using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class ApplicationDetailsDto
    {
        public IEnumerable<StandardsListItemDto> Standards { get; set; }
        public OpportunityDto Opportunity { get; set; }
    }
}