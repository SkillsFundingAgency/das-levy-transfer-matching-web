using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SectorViewModel : SectorPostRequest
    {
        public List<ReferenceDataItem> SectorOptions { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
    }
}
