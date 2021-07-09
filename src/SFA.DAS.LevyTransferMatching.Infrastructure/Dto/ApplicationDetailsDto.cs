using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class ApplicationDetailsDto
    {
        public IEnumerable<StandardsListItemDto> Standards { get; set; }
        public OpportunityDto Opportunity { get; set; }
    }
}
