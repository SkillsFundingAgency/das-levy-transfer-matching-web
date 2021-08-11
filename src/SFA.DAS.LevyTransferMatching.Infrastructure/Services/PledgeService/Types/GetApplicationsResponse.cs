using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationsResponse
    {
        public StandardsListItemDto Standard { get; set; }
        public IEnumerable<ApplicationDto> Applications { get; set; }
    }
}
