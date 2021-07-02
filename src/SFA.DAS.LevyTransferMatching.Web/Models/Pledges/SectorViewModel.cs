using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorViewModel : SectorPostRequest
    {
        public List<ReferenceDataItem> SectorOptions { get; set; }
    }
}
