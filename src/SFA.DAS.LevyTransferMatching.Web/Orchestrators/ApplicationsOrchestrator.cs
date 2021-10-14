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
        private readonly Infrastructure.Configuration.FeatureToggles _featureToggles;

        public ApplicationsOrchestrator(IApplicationsService applicationsService, IEncodingService encodingService, Infrastructure.Configuration.FeatureToggles featureToggles)
        {
            _applicationsService = applicationsService;
            _encodingService = encodingService;
            _featureToggles = featureToggles;
        }

        public async Task<GetApplicationsViewModel> GetApplications(GetApplicationsRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _applicationsService.GetApplications(request.AccountId, cancellationToken);

            var applicationViewModels = result.Applications?.Select(app =>
            {
                var duration = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).Duration;
                return new GetApplicationsViewModel.ApplicationViewModel
                {
                    EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                    DasAccountName = app.IsNamePublic ? app.DasAccountName : "Opportunity",
                    Amount = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate)
                        .CalcFundingForDate(app.NumberOfApprentices, app.StartDate),
                    Duration = duration,
                    CreatedOn = app.CreatedOn,
                    Status = app.Status,
                    NumberOfApprentices = app.NumberOfApprentices,
                    PledgeReference = _encodingService.Encode(app.PledgeId, EncodingType.PledgeId),
                    IsNamePublic = app.IsNamePublic,
                    EstimatedTotalCost = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).CalculateEstimatedTotalCost(app.NumberOfApprentices).ToString("N0")
                };
            }).ToList();

            var viewModel = new GetApplicationsViewModel()
            {
                Applications = applicationViewModels,
                EncodedAccountId = request.EncodedAccountId,
                RenderViewApplicationDetailsHyperlink = _featureToggles.FeatureToggleCanViewApplicationDetails
            };

            return viewModel;
        }
    }
}
