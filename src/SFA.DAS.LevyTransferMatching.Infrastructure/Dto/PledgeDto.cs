using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class PledgeDto
    {
        public int Amount { get; set; }
        public bool IsNamePublic { get; set; }
        public string DasAccountName { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
        public List<string> Locations { get; set; }
    }
}
