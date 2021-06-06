using SFA.DAS.LevyTransferMatching.Infrastructure.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelPostRequest : PledgesRequest
    {
        public Level? Levels { get; set; }
    }
}