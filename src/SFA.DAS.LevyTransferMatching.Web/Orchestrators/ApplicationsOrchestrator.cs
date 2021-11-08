using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class ApplicationsOrchestrator : OpportunitiesOrchestratorBase, IApplicationsOrchestrator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IEncodingService _encodingService;
        private readonly Infrastructure.Configuration.FeatureToggles _featureToggles;
        private readonly IUserService _userService;

        public ApplicationsOrchestrator(IApplicationsService applicationsService, IDateTimeService dateTimeService, IEncodingService encodingService, 
            Infrastructure.Configuration.FeatureToggles featureToggles, IUserService userService) : base(dateTimeService)
        {
            _applicationsService = applicationsService;
            _encodingService = encodingService;
            _featureToggles = featureToggles;
            _userService = userService;
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

        public async Task<ApplicationViewModel> GetApplication(ApplicationRequest request)
        {
           var result = await _applicationsService.GetApplication(request.AccountId, request.ApplicationId);

            if (result == null)
            {
                return null;
            }

            var isOwnerOrTransactor = _userService.IsOwnerOrTransactor(request.AccountId);
            var encodedOpportunityId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);
            var encodedSenderPublicAccountId = _encodingService.Encode(result.SenderEmployerAccountId, EncodingType.PublicAccountId);

            var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions()
            {
                Sectors = result.Sectors,
                JobRoles = result.JobRoles,
                Levels = result.Levels,
                Locations = result.PledgeLocations,
                AllSectors = result.AllSectors,
                AllJobRoles = result.AllJobRoles,
                AllLevels = result.AllLevels,
                Amount = result.RemainingAmount,
                IsNamePublic = result.IsNamePublic,
                DasAccountName = result.PledgeEmployerAccountName,
                EncodedPledgeId = encodedOpportunityId,
            };

            var estimatedTotalCost = result.Standard.ApprenticeshipFunding
                .GetEffectiveFundingLine(result.StartBy)
                .CalculateEstimatedTotalCost(result.NumberOfApprentices);

            return new ApplicationViewModel()
            {
                 OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
                 PledgeEmployerAccountName = result.PledgeEmployerAccountName,
                 EncodedAccountId = request.EncodedAccountId,
                 EncodedApplicationId = request.EncodedApplicationId,
                 IsNamePublic = result.IsNamePublic,
                 JobRole = result.Standard.Title,
                 Level = result.Standard.Level,
                 Locations = result.PledgeLocations,
                 NumberOfApprentices = result.NumberOfApprentices,
                 StartBy = result.StartBy,
                 Status = result.Status,
                 EncodedOpportunityId = encodedOpportunityId,
                 EstimatedTotalCost = estimatedTotalCost.ToCurrencyString(),
                 CanAcceptFunding = isOwnerOrTransactor && result.Status == ApplicationStatus.Approved,
                 CanUseTransferFunds = isOwnerOrTransactor && result.Status == ApplicationStatus.Accepted,
                 EncodedSenderPublicAccountId = encodedSenderPublicAccountId,
                 RenderCanUseTransferFundsStartButton = _featureToggles.FeatureToggleRenderCanUseTransferFundsStartButton,
                 DisplayCurrentFundsBalance = result.AmountUsed > 0 || result.NumberOfApprenticesUsed > 0,
                 AmountUsed = result.AmountUsed.ToCurrencyString(),
                 AmountRemaining = (estimatedTotalCost - result.AmountUsed) < 0 ? 0.ToCurrencyString() : (estimatedTotalCost - result.AmountUsed).ToCurrencyString(),
                 NumberOfApprenticesRemaining = (result.NumberOfApprentices - result.NumberOfApprenticesUsed) < 0 ? 0 : (result.NumberOfApprentices - result.NumberOfApprenticesUsed)
            };
        }

        public async Task SetApplicationAcceptance(ApplicationPostRequest request)
        {
            await _applicationsService.SetApplicationAcceptance(new SetApplicationAcceptanceRequest
            {
                ApplicationId = request.ApplicationId,
                AccountId = request.AccountId,
                UserDisplayName = _userService.GetUserDisplayName(),
                UserId = _userService.GetUserId(),
                Acceptance = request.SelectedAction == ApplicationViewModel.ApprovalAction.Accept ?
                    SetApplicationAcceptanceRequest.ApplicationAcceptance.Accept
                    : SetApplicationAcceptanceRequest.ApplicationAcceptance.Decline,
            });
        }

        public async Task<AcceptedViewModel> GetAcceptedViewModel(AcceptedRequest request)
        {
            var result = await _applicationsService.GetAccepted(request.AccountId, request.ApplicationId);

            if (result == null)
            {
                return null;
            }

            var encodedPledgeId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);

            return new AcceptedViewModel()
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedApplicationId = request.EncodedApplicationId,
                EmployerNameAndReference = $"{result.EmployerAccountName} ({encodedPledgeId})",
            };
        }
    }
}
