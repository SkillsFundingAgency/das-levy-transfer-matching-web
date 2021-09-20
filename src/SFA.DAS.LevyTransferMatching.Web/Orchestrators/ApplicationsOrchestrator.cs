using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class ApplicationsOrchestrator : IApplicationsOrchestrator
    {
        public async Task<GetApplicationsViewModel> GetApplications(string hashedAccountId)
        {
            var viewModel = new GetApplicationsViewModel()
            {

            };

            return viewModel;
        }
    }
}
