using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class ApplicationViewModelExtensions
    {
        public static int GetCriteriaScore(this ApplicationViewModel applicationViewModel)
        {
            int criteriaScore = 0;
            if (applicationViewModel.IsJobRoleMatch)
                criteriaScore++;
            if (applicationViewModel.IsLevelMatch)
                criteriaScore++;
            if (applicationViewModel.IsLocationMatch)
                criteriaScore++;
            if (applicationViewModel.IsSectorMatch)
                criteriaScore++;

            return criteriaScore;
        }
    }
}
