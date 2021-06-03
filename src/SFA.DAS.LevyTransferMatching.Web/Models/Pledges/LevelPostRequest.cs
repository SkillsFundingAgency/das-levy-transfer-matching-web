using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelPostRequest : PledgesRequest
    {
        public Level? Levels { get; set; }
    }
}