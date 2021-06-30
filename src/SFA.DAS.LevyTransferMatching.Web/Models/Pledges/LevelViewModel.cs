using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelViewModel : LevelPostRequest
    {
        public List<ReferenceDataItem> LevelOptions { get; set; }
    }
}