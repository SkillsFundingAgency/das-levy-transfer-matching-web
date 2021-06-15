using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorViewModel : SectorPostRequest
    {
        public List<Tag> SectorOptions { get; set; }
    }
}
