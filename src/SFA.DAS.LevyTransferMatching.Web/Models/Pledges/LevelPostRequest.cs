using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LevelPostRequest : BasePledgesRequest
    {
        public List<string> Levels { get; set; }
    }
}