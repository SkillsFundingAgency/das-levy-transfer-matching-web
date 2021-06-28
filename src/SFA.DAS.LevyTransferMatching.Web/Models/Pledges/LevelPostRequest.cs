using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelPostRequest : PledgesRequest
    {
        public List<string> Levels { get; set; }
    }
}