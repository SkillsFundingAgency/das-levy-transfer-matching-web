using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class ApplicationsOrchestrator : OpportunitiesOrchestratorBase, IApplicationsOrchestrator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IEncodingService _encodingService;

        public ApplicationsOrchestrator(IApplicationsService applicationsService, IDateTimeService dateTimeService, IEncodingService encodingService) : base(dateTimeService)
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
                Amount = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).CalcFundingForDate(app.NumberOfApprentices, app.StartDate),
                Duration = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).Duration,
                CreatedOn = app.CreatedOn,
                Status = app.Status,
                NumberOfApprentices = app.NumberOfApprentices,
                PledgeReference = _encodingService.Encode(app.Id, EncodingType.PledgeId),
                IsAnonymous = app.IsAnonymousPledge
            }).ToList();

            var viewModel = new GetApplicationsViewModel()
            {
                Applications = applicationViewModels,
                EncodedAccountId = request.EncodedAccountId
            };

            return viewModel;
        }

        public async Task<ApplicationStatusViewModel> GetApplicationStatusViewModel(ApplicationStatusRequest request)
        {
            var result = await _applicationsService.GetApplicationStatus(request.AccountId, request.ApplicationId);

            var encodedOpportunityId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);

            return new ApplicationStatusViewModel()
            {
                 OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(result.Sectors, result.JobRoles, result.Levels, result.PledgeLocations, result.AllSectors, result.AllJobRoles, result.AllLevels, result.Amount, result.IsNamePublic, result.EmployerAccountName, encodedOpportunityId),
                 Amount = result.Amount.ToCurrencyString(),
                 EmployerAccountName = result.EmployerAccountName,
                 EncodedAccountId = request.EncodedAccountId,
                 EncodedApplicationId = request.EncodedApplicationId,
                 IsNamePublic = result.IsNamePublic,
                 JobRole = result.JobRole,
                 Level = result.Level,
                 Locations = result.PledgeLocations,
                 NumberOfApprentices = result.NumberOfApprentices,
                 StartBy = result.StartBy,
                 Status = result.Status,
                 EncodedOpportunityId = encodedOpportunityId,
            };
        }
    }
}
