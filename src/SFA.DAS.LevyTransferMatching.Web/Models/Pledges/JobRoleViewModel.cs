using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRoleViewModel : JobRolePostRequest
    {
        public List<string> Sectors { get; set; }
        public List<ReferenceDataItem> JobRoleOptions { get; set; }
        public List<ReferenceDataItem> SectorOptions { get; set; }
        public bool AreAllSectorsSelected => Sectors == null || !Sectors.Any() || Sectors.Count == SectorOptions.Count;
    }
}
