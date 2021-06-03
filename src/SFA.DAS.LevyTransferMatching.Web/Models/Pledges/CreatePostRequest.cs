using SFA.DAS.LevyTransferMatching.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreatePostRequest : PledgesRequest
    {
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        public Sector? Sectors { get; set; }
        public JobRole? JobRoles { get; set; }
        public Level? Levels { get; set; }
    }
}
