using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelViewModel : LevelPostRequest
    {
        public List<Tag> LevelOptions { get; set; }
    }
}