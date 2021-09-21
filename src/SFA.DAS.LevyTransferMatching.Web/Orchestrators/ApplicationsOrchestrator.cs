using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class ApplicationsOrchestrator : IApplicationsOrchestrator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IEncodingService _encodingService;

        public ApplicationsOrchestrator(IApplicationsService applicationsService, IEncodingService encodingService)
        {
            _applicationsService = applicationsService;
            _encodingService = encodingService;
        }

        public async Task<GetApplicationsViewModel> GetApplications(GetApplicationsRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _applicationsService.GetApplications(request.AccountId, cancellationToken);

            var applicationViewModels = result.Applications?.Select(app => new GetApplicationsViewModel.ApplicationViewModel
            {
                EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                DasAccountName = app.DasAccountName,
                Amount = app.Amount,
               // Duration = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).Duration,
                CreatedOn = app.CreatedOn,
                Status = app.Status,
                NumberOfApprentices = app.NumberOfApprentices,
            }).ToList();

            var viewModel = new GetApplicationsViewModel()
            {
                Applications = applicationViewModels
            };

            return viewModel;
        }
    }
}
