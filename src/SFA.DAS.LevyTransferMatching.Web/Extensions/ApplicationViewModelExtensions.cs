using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class ApplicationViewModelExtensions
    {
        public static string CalculateMatchPercentage(this ApplicationViewModel viewModel)
        {
            int matches = 0;
            if (viewModel.IsLocationMatch) matches++;
            if (viewModel.IsSectorMatch) matches++;
            if (viewModel.IsJobRoleMatch) matches++;
            if (viewModel.IsLevelMatch) matches++;
            switch (matches)
            {
                case 0: return "0%";
                case 1: return "25%";
                case 2: return "50%";
                case 3: return "75%";
                case 4: return "100%";
                default: return "0%";
            }
        }

        public static string GetMatchPercentageCss(this ApplicationViewModel viewModel)
        {
            int matches = 0;
            if (viewModel.IsLocationMatch) matches++;
            if (viewModel.IsSectorMatch) matches++;
            if (viewModel.IsJobRoleMatch) matches++;
            if (viewModel.IsLevelMatch) matches++;
            switch (matches)
            {
                case 0: case 1: return "pink";
                case 2: case 3: return "yellow";
                case 4: return "turquoise";
                default: return "pink";
            }
        }
    }
}
