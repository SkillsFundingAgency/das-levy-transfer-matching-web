using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class PledgeDto
    {
        public string AccountId { get; set; }
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        //public Sector? Sectors { get; set; }
        //public JobRole? JobRoles { get; set; }
        //public Level? Levels { get; set; }
    }
}
