using SFA.DAS.LevyTransferMatching.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class PledgeDto
    {
        public int Amount { get; set; }
        public bool IsNamePublic { get; set; }
        public List<Sector> Sectors { get; set; }
        public List<JobRole> JobRoles { get; set; }
        public List<Level> Levels { get; set; }
    }
}
