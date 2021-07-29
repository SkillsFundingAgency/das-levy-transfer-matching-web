using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SectorViewModel : SectorPostRequest
    {
        public List<ReferenceDataItem> SectorOptions { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
    }
}