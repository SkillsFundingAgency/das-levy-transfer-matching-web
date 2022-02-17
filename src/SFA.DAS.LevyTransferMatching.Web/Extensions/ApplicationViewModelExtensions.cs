using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class ApplicationViewModelExtensions
    {
        public static string GetMatchPercentageCss(this ApplicationViewModel viewModel)
        {
            switch (viewModel.MatchPercentage)
            {
                case 50: case 75: return "yellow";
                case 100: return "turquoise";
                case 0: case 25: default: return "pink";
            }
        }
    }
}
