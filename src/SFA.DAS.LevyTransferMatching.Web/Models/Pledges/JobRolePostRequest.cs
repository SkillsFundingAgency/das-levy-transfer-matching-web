﻿using SFA.DAS.LevyTransferMatching.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRolePostRequest : PledgesRequest
    {
        public JobRole? JobRoles { get; set; }
    }
}
