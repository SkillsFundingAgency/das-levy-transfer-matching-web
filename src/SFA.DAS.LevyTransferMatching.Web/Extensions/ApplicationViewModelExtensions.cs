using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class ApplicationViewModelExtensions
{
    public static int GetCriteriaScore(this ApplicationsViewModel.Application applicationViewModel)
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