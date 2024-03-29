﻿using System.Dynamic;
using Humanizer;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Services;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public class PledgeOrchestrator : IPledgeOrchestrator
{
    private readonly IPledgeService _pledgeService;
    private readonly IEncodingService _encodingService;
    private readonly IUserService _userService;
    private readonly Infrastructure.Configuration.FeatureToggles _featureToggles;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICsvHelperService _csvService;

    public PledgeOrchestrator(IPledgeService pledgeService, IEncodingService encodingService, IUserService userService, Infrastructure.Configuration.FeatureToggles featureToggles, IDateTimeService dateTimeService, ICsvHelperService csvService)
    {
        _pledgeService = pledgeService;
        _encodingService = encodingService;
        _userService = userService;
        _featureToggles = featureToggles;
        _dateTimeService = dateTimeService;
        _csvService = csvService;
    }

    public CloseViewModel GetCloseViewModel(CloseRequest request)
    {
        return new CloseViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            CacheKey = Guid.NewGuid(),
            UserCanClosePledge = _userService.IsUserChangeAuthorized(request.EncodedAccountId)
        };
    }

    public async Task<PledgesViewModel> GetPledgesViewModel(PledgesRequest request)
    {
        var pledgesResponse = await _pledgeService.GetPledges(request.AccountId);
        var renderCreatePledgesButton = _userService.IsUserChangeAuthorized(request.EncodedAccountId);

        return new PledgesViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            RenderCreatePledgeButton = renderCreatePledgesButton,
            Pledges = pledgesResponse.Pledges.Select(x => new PledgesViewModel.Pledge
            {
                ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                Amount = x.Amount,
                RemainingAmount = x.RemainingAmount,
                ApplicationCount = x.ApplicationCount,
                Status = x.Status
            })
        };
    }

    public DetailViewModel GetDetailViewModel(DetailRequest request)
    {
        return new DetailViewModel
        {
            EncodedPledgeId = request.EncodedPledgeId
        };
    }

    public async Task ClosePledge(ClosePostRequest request)
    {
        var closePledgeRequest = new ClosePledgeRequest
        {
            UserId = _userService.GetUserId(),
            UserDisplayName = _userService.GetUserDisplayName()
        };

        await _pledgeService.ClosePledge(request.AccountId, request.PledgeId, closePledgeRequest);
    }

    public async Task<ApplicationApprovedViewModel> GetApplicationApprovedViewModel(ApplicationApprovedRequest request)
    {
        var response = await _pledgeService.GetApplicationApproved(request.AccountId, request.PledgeId, request.ApplicationId);

        return new ApplicationApprovedViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            DasAccountName = response.EmployerAccountName,
            AllowTransferRequestAutoApproval = response.AutomaticApproval
        };
    }


    public async Task<byte[]> GetPledgeApplicationsDownloadModel(ApplicationsRequest request)
    {
        var result = await _pledgeService.GetApplications(request.AccountId, request.PledgeId, request.SortColumn, request.SortOrder);

        var pledgeAppModel = new PledgeApplicationsDownloadModel
        {
            Applications = result.Applications?.Select(app => new PledgeApplicationDownloadModel
            {
                DateApplied = app.CreatedOn,
                Status = app.GetDateDependentStatus(result.AutomaticApprovalOption),
                ApplicationId = app.Id,
                PledgeId = app.PledgeId,
                EmployerAccountName = app.DasAccountName,
                HasTrainingProvider = app.HasTrainingProvider,
                Sectors = app.Sectors ?? new List<string>(),
                AboutOpportunity = app.Details,
                BusinessWebsite = GetUrlWithPrefix(app.BusinessWebsite),
                FormattedEmailAddress = string.Join(";", app.EmailAddresses),
                FormattedSectors = string.Join(",", app.Sectors ?? new List<string>()),
                FirstName = app.FirstName,
                LastName = app.LastName,
                NumberOfApprentices = app.NumberOfApprentices,
                StartBy = app.StartDate,
                TypeOfJobRole = app.JobRole,
                EncodedPledgeId = _encodingService.Encode(app.PledgeId, EncodingType.PledgeId),
                EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                MatchJobRole = app.IsJobRoleMatch,
                MatchLevel = app.IsLevelMatch,
                MatchLocation = app.IsLocationMatch,
                MatchSector = app.IsSectorMatch,
                Duration = app.StandardDuration,
                EstimatedCostThisYear = app.Amount,
                Level = app.Level,
                TotalEstimatedCost = app.MaxFunding,
                AdditionalLocations = app.AdditionalLocations,
                SpecificLocation = app.SpecificLocation,
                Locations = app.Locations,
                PledgeLocations = app.PledgeLocations,
                DynamicLocations = GetListOfLocations(app.PledgeLocations, app.Locations, app.SpecificLocation, app.AdditionalLocations)
            })
        };

        var fileContents = _csvService.GenerateCsvFileFromModel(pledgeAppModel);

        return fileContents;
    }
               
    private static IEnumerable<dynamic> GetListOfLocations(IEnumerable<GetApplyResponse.PledgeLocation> pledgeLocations, IEnumerable<GetApplicationsResponse.ApplicationLocation> applicationLocations, string specificLocation, string additionalLocations)
    {
        var listOfMatchingLocations = (from location in applicationLocations
            select pledgeLocations?.FirstOrDefault(o => o.Id == location.PledgeLocationId)
            into matchedLocation
            where matchedLocation != null && !string.IsNullOrWhiteSpace(matchedLocation.Name)
            select matchedLocation.Name).ToList();

        if (!string.IsNullOrWhiteSpace(specificLocation))
        {
            listOfMatchingLocations.Add(specificLocation);
        }

        if (!string.IsNullOrWhiteSpace(additionalLocations))
        {
            listOfMatchingLocations.Add(additionalLocations);
        }

        var locations = new List<dynamic>();

        foreach (var matchingLocation in listOfMatchingLocations)
        {
            dynamic location = new ExpandoObject();
            location.Name = matchingLocation;
            locations.Add(location);
        }

        return locations;
    }

    public async Task<ApplicationsViewModel> GetApplications(ApplicationsRequest request)
    {
        var result = await _pledgeService.GetApplications(request.AccountId, request.PledgeId, request.SortColumn, request.SortOrder);

        var isOwnerOrTransactor = _userService.IsOwnerOrTransactor(request.EncodedAccountId);

        var viewModels = (from application in result.Applications
            let pledgeApplication = result.Applications.First(x => x.PledgeId == application.PledgeId)
            select new ApplicationsViewModel.Application
            {
                EncodedApplicationId = _encodingService.Encode(application.Id, EncodingType.PledgeApplicationId),
                DasAccountName = application.DasAccountName,
                Amount = application.Amount,
                Duration = application.StandardDuration,
                CreatedOn = application.CreatedOn,
                Status = application.Status,
                IsLocationMatch = application.IsLocationMatch,
                IsSectorMatch = application.IsSectorMatch,
                IsJobRoleMatch = application.IsJobRoleMatch,
                IsLevelMatch = application.IsLevelMatch,
                StartBy = application.StartDate,
                BusinessWebsite = pledgeApplication.BusinessWebsite,
                LastName = pledgeApplication.LastName,
                FirstName = pledgeApplication.FirstName,
                EmailAddresses = pledgeApplication.EmailAddresses,
                JobRole = pledgeApplication.JobRole,
                PledgeRemainingAmount = pledgeApplication.PledgeRemainingAmount,
                MaxFunding = pledgeApplication.MaxFunding,
                Details = pledgeApplication.Details,
                RemainingDaysForDelayedApproval = application.GetRemainingDaysForDelayedApproval(result.AutomaticApprovalOption),
                RemainingDaysForAutoRejection = application.GetRemainingDaysForAutoRejection()
            }).ToList();

        return new ApplicationsViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            UserCanClosePledge = result.PledgeStatus != PledgeStatus.Closed && isOwnerOrTransactor,
            EncodedPledgeId = request.EncodedPledgeId,
            RenderCreatePledgeButton = isOwnerOrTransactor,
            RenderRejectButton = viewModels.Any(x => x.Status == ApplicationStatus.Pending),
            PledgeTotalAmount = result.PledgeTotalAmount.ToCurrencyString(),
            AutomaticApprovalOption = result.AutomaticApprovalOption,
            PledgeRemainingAmount = result.PledgeRemainingAmount.ToCurrencyString(),
            Applications = viewModels
                .OrderBy(app => app.RemainingDaysForDelayedApproval.GetValueOrDefault(int.MaxValue))
                .ThenBy(app => app.RemainingDaysForAutoRejection.GetValueOrDefault(int.MaxValue))
                .ToList()
        };
    }

    public async Task<ApplicationViewModel> GetApplicationViewModel(ApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var result =
            await _pledgeService.GetApplication(request.AccountId, request.PledgeId, request.ApplicationId, cancellationToken);

        var isOwnerOrTransactor = _userService.IsOwnerOrTransactor(request.EncodedAccountId);

        if (result != null)
        {
            return new ApplicationViewModel
            {
                AboutOpportunity = result.AboutOpportunity,
                BusinessWebsite = GetUrlWithPrefix(result.BusinessWebsite),
                EmailAddresses = result.EmailAddresses,
                CreatedOn = result.CreatedOn,
                EmployerAccountName = result.EmployerAccountName,
                EstimatedDurationMonths = result.EstimatedDurationMonths,
                FirstName = result.FirstName,
                HasTrainingProvider = result.HasTrainingProvider,
                LastName = result.LastName,
                NumberOfApprentices = result.NumberOfApprentices,
                StartBy = result.StartBy,
                Sectors = result.Sector,
                AllSectors = result.AllSectors,
                PledgeSectors = result.PledgeSectors,
                PledgeJobRoles = result.PledgeJobRoles,
                PledgeLevels = result.PledgeLevels,
                PledgeLocations = result.PledgeLocations,
                JobRole = result.TypeOfJobRole,
                Level = result.Level,
                DisplaySectors = result.Sector.ToReferenceDataDescriptionList(result.AllSectors, "; "),
                Locations = string.IsNullOrEmpty(result.SpecificLocation) ? result.Locations.ToApplicationLocationsString(", ", result.AdditionalLocation) : result.SpecificLocation,
                IsJobRoleMatch = result.IsJobRoleMatch,
                IsLevelMatch = result.IsLevelMatch,
                IsLocationMatch = result.IsLocationMatch,
                IsSectorMatch = result.IsSectorMatch,
                MatchPercentage = result.MatchPercentage,
                Affordability = GetAffordabilityViewModel(result.PledgeRemainingAmount, result.NumberOfApprentices, result.MaxFunding, result.EstimatedDurationMonths, result.StartBy),
                AllowApproval = result.Status == ApplicationStatus.Pending && result.Amount <= result.PledgeRemainingAmount && isOwnerOrTransactor,
                AllowRejection = result.Status == ApplicationStatus.Pending && isOwnerOrTransactor,
                DisplayApplicationApprovalOptions = _featureToggles.FeatureToggleApplicationApprovalOptions,
                Status = result.Status
            };
        }

        return null;
    }

    public async Task<ApplicationApprovalOptionsViewModel> GetApplicationApprovalOptionsViewModel(ApplicationApprovalOptionsRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _pledgeService.GetApplicationApprovalOptions(request.AccountId, request.PledgeId, request.ApplicationId, cancellationToken);

        return new ApplicationApprovalOptionsViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            EncodedApplicationId = request.EncodedApplicationId,
            EmployerAccountName = response.EmployerAccountName,
            IsApplicationPending = response.ApplicationStatus == ApplicationStatus.Pending
        };
    }

    public async Task SetApplicationApprovalOptions(ApplicationApprovalOptionsPostRequest request, CancellationToken cancellationToken = default)
    {
        var serviceRequest = new SetApplicationApprovalOptionsRequest
        {
            UserId = _userService.GetUserId(),
            UserDisplayName = _userService.GetUserDisplayName(),
            AutomaticApproval = request.AutomaticApproval.Value
        };

        await _pledgeService.SetApplicationApprovalOptions(request.AccountId, request.ApplicationId, request.PledgeId, serviceRequest);
    }

    public async Task SetApplicationOutcome(ApplicationPostRequest request)
    {
        var outcomeRequest = new SetApplicationOutcomeRequest
        {
            UserId = _userService.GetUserId(),
            UserDisplayName = _userService.GetUserDisplayName(),
            Outcome = request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve
                ? SetApplicationOutcomeRequest.ApplicationOutcome.Approve
                : SetApplicationOutcomeRequest.ApplicationOutcome.Reject
        };

        await _pledgeService.SetApplicationOutcome(request.AccountId, request.ApplicationId, request.PledgeId, outcomeRequest);
    }

    private static string GetUrlWithPrefix(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;

        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            return url;
        }

        return $"http://{url}";
    }

    public ApplicationViewModel.AffordabilityViewModel GetAffordabilityViewModel(int remainingAmount, int numberOfApprentices, int maxFunding, int estimatedDurationMonths, DateTime startDate)
    {
        var totalCost = maxFunding * numberOfApprentices;

        var yearlyBreakdown = CalculateYearlyPayments(totalCost, estimatedDurationMonths).ToList();

        return new ApplicationViewModel.AffordabilityViewModel
        {
            EstimatedCostOverDuration = totalCost,
            YearlyPayments = yearlyBreakdown,
            YearDescription = _dateTimeService.UtcNow.ToTaxYearDescription(),
            RemainingFundsIfApproved = remainingAmount - (int) yearlyBreakdown.First().Amount
        };
    }

    private static IEnumerable<YearlyPayments> CalculateYearlyPayments(decimal totalAmount, int durationInMonths)
    {
        if (durationInMonths <= 12)
        {
            return new List<YearlyPayments> { new(string.Empty, (int) totalAmount) };
        }

        var completionPayment = totalAmount / 5;

        var yearlyPayments = new List<YearlyPayments>();

        var years = durationInMonths / 12;
        var months = durationInMonths % 12;

        var paymentPerMonth = (totalAmount - completionPayment) / durationInMonths;

        for (var i = 0; i < years; i++)
        {
            if ((i == years - 1) && (months == 0))
            {
                var finalYearAmount = (int)Math.Round(paymentPerMonth * 12);
                finalYearAmount += (int)completionPayment;
                yearlyPayments.Add(new YearlyPayments("final year", finalYearAmount));
            }
            else
            {
                var yearAmount = (int)Math.Round(paymentPerMonth * 12);
                var yearLabel = $"{(i + 1).ToOrdinalWords()} year";
                yearlyPayments.Add(new YearlyPayments(yearLabel, yearAmount));
            }
        }

        if (months > 0)
        {
            var finalYearAmount = (int)Math.Round(paymentPerMonth * months);
            finalYearAmount += (int) completionPayment;
            yearlyPayments.Add(new YearlyPayments("final year", finalYearAmount));
        }

        return yearlyPayments;
    }

    public async Task RejectApplications(RejectApplicationsPostRequest request)
    {
        var serviceRequest = new SetRejectApplicationsRequest
        {
            UserId = _userService.GetUserId(),
            UserDisplayName = _userService.GetUserDisplayName(),
            ApplicationsToReject = request.ApplicationsToReject.Select(applicationId => (int)_encodingService.Decode(applicationId,
                EncodingType.PledgeApplicationId)).ToList()
        };

        await _pledgeService.RejectApplications(serviceRequest, request.AccountId, request.PledgeId);
    }

    public async Task<RejectApplicationsViewModel> GetRejectApplicationsViewModel(RejectApplicationsRequest request)
    {
        var applicationsList = await _pledgeService.GetRejectApplications(request.AccountId, request.PledgeId);

        return new RejectApplicationsViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            DasAccountNames = applicationsList.Applications.Where(x =>
                    request.ApplicationsToReject.Any((s =>
                        (int)_encodingService.Decode(s, EncodingType.PledgeApplicationId) == x.Id)))
                .Select(app => app.DasAccountName).ToList()
        };
    }
}