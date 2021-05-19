using Microsoft.AspNetCore.Routing;
using SFA.DAS.LevyTransferMatching.Web.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgesOrchestrator
    {
        public IndexViewModel Index(RouteData routeData)
        {
            var indexViewModel = this.CreateViewModel<IndexViewModel>(routeData);

            return indexViewModel;
        }

        public CreateViewModel Create(RouteData routeData)
        {
            var createViewModel = this.CreateViewModel<CreateViewModel>(routeData);

            return createViewModel;
        }

        private TPledgesViewModel CreateViewModel<TPledgesViewModel>(RouteData routeData) where TPledgesViewModel : PledgesViewModel, new()
        {
            string encodedAccountId = (string)routeData.Values[RouteValueKeys.EncodedAccountId];

            TPledgesViewModel pledgesViewModel = new TPledgesViewModel()
            {
                EncodedAccountId = encodedAccountId,
            };

            return pledgesViewModel;
        }
    }
}