using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private const int MaximumNumberAdditionalEmailAddresses = 4;

        private readonly ICacheStorageService _cacheStorageService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly IEncodingService _encodingService;
        private readonly IUserService _userService;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, IUserService userService, IEncodingService encodingService, ICacheStorageService cacheStorageService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _encodingService = encodingService;
            _userService = userService;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<DetailViewModel> GetDetailViewModel(int pledgeId)
        {
            var response = await _opportunitiesService.GetDetail(pledgeId);

            if (response.Opportunity == null)
                return null;

            var encodedPledgeId = _encodingService.Encode(response.Opportunity.Id, EncodingType.PledgeId);

            var opportunitySummaryViewModel = GetOpportunitySummaryViewModel
                (
                    response.Opportunity.Sectors,
                    response.Opportunity.JobRoles,
                    response.Opportunity.Levels,
                    response.Sectors,
                    response.JobRoles,
                    response.Levels,
                    response.Opportunity.Amount,
                    response.Opportunity.IsNamePublic,
                    response.Opportunity.DasAccountName,
                    encodedPledgeId
                );

            return new DetailViewModel()
            {
                EmployerName = response.Opportunity.DasAccountName,
                EncodedPledgeId = encodedPledgeId,
                IsNamePublic = response.Opportunity.IsNamePublic,
                OpportunitySummaryView = opportunitySummaryViewModel,
            };
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var response = await _opportunitiesService.GetIndex();

            return new IndexViewModel 
            { 
                Opportunities = response?.Opportunities
                    .Select(x => new IndexViewModel.Opportunity
                    {
                        Amount = x.Amount,
                        EmployerName = x.DasAccountName,
                        ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                        Sectors = x.Sectors.ToReferenceDataDescriptionList(response.Sectors),
                        JobRoles = x.JobRoles.ToReferenceDataDescriptionList(response.JobRoles),
                        Levels = x.Levels.ToReferenceDataDescriptionList(response.Levels, descriptionSource: y => y.ShortDescription),
                        Locations = x.Locations
                    }).ToList()
            };
        }

        public async Task<string> GetUserEncodedAccountId()
        {
            var userAccounts = await _userService.GetLoggedInUserAccounts();

            // TODO: Below is temporary -
            //       Raised as an issue, and eventually to be replaced with
            //       an accounts selection screen.
            var firstEncodedAccountId = userAccounts
                .Select(x => x.EncodedAccountId)
                .First();

            return firstEncodedAccountId;
        }

        public async Task<ConfirmationViewModel> GetConfirmationViewModel(ConfirmationRequest request)
        {
            var result = await _opportunitiesService.GetConfirmation(request.AccountId, request.PledgeId);
            return new ConfirmationViewModel
            {
                AccountName = result.AccountName,
                IsNamePublic = result.IsNamePublic,
                Reference = _encodingService.Encode(request.PledgeId, EncodingType.PledgeId),
                EncodedAccountId = request.EncodedAccountId
            };
        }

        public async Task SubmitApplication(ApplyPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            var applyRequest = new Infrastructure.Services.OpportunitiesService.Types.ApplyRequest
            {
                EncodedAccountId = request.EncodedAccountId,
                Details = cacheItem.Details ?? string.Empty,
                StandardId = cacheItem.StandardId,
                NumberOfApprentices = cacheItem.NumberOfApprentices.Value,
                StartDate = cacheItem.StartDate.Value,
                HasTrainingProvider = cacheItem.HasTrainingProvider.Value,
                Sectors = cacheItem.Sectors,
                Postcode = cacheItem.Postcode ?? string.Empty,
                FirstName = cacheItem.FirstName ?? string.Empty,
                LastName = cacheItem.LastName ?? string.Empty,
                EmailAddresses = cacheItem.EmailAddresses,
                BusinessWebsite = cacheItem.BusinessWebsite ?? string.Empty
            };

            await _opportunitiesService.PostApplication(request.AccountId, request.PledgeId, applyRequest);

            await _cacheStorageService.DeleteFromCache(request.CacheKey.ToString());
        }

        public OpportunitySummaryViewModel GetOpportunitySummaryViewModel(
            IEnumerable<string> sectors,
            IEnumerable<string> jobRoles,
            IEnumerable<string> levels,
            IEnumerable<ReferenceDataItem> allSectors,
            IEnumerable<ReferenceDataItem> allJobRoles,
            IEnumerable<ReferenceDataItem> allLevels,
            int amount,
            bool isNamePublic,
            string dasAccountName,
            string encodedPledgeId)
        {
            string sectorList = sectors.ToReferenceDataDescriptionList(allSectors);
            string jobRoleList = jobRoles.ToReferenceDataDescriptionList(allJobRoles);
            string levelList = levels.ToReferenceDataDescriptionList(allLevels, descriptionSource: x => x.ShortDescription);

            DateTime dateTime = _dateTimeService.UtcNow;

            return new OpportunitySummaryViewModel()
            {
                Amount = amount,
                Description = isNamePublic ? $"{dasAccountName} ({encodedPledgeId})" : "A levy-paying business",
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                YearDescription = dateTime.ToTaxYearDescription(),
            };
        }

        public async Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request)
        {
            var applicationTask = RetrieveCacheItem(request.CacheKey);
            var applyResponseTask = _opportunitiesService.GetApply(request.AccountId, request.PledgeId);

            await Task.WhenAll(applicationTask, applyResponseTask);

            var contactName = $"{applicationTask.Result.FirstName} {applicationTask.Result.LastName}";

            return new ApplyViewModel
            {
                CacheKey = applicationTask.Result.Key,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedAccountId = request.EncodedAccountId,
                OpportunitySummaryViewModel = GetOpportunitySummaryViewModel
                    (
                        applyResponseTask.Result.Opportunity.Sectors,
                        applyResponseTask.Result.Opportunity.JobRoles,
                        applyResponseTask.Result.Opportunity.Levels,
                        applyResponseTask.Result.Sectors,
                        applyResponseTask.Result.JobRoles,
                        applyResponseTask.Result.Levels,
                        applyResponseTask.Result.Opportunity.Amount,
                        applyResponseTask.Result.Opportunity.IsNamePublic,
                        applyResponseTask.Result.Opportunity.DasAccountName,
                        request.EncodedPledgeId
                    ),
                JobRole = applicationTask.Result.JobRole ?? "-",
                NumberOfApprentices = applicationTask.Result.NumberOfApprentices.HasValue ? applicationTask.Result.NumberOfApprentices.Value.ToString() : "-",
                StartBy = applicationTask.Result.StartDate.HasValue ? applicationTask.Result.StartDate.Value.ToShortDisplayString() : "-",
                HaveTrainingProvider = applicationTask.Result.HasTrainingProvider.ToApplyViewString(),
                Sectors = applicationTask.Result.Sectors?.ToList(),
                SectorOptions = applyResponseTask.Result.Sectors?.ToList(),
                Location = applicationTask.Result.Postcode ?? "-",
                MoreDetail = applicationTask.Result.Details ?? "-",
                ContactName = string.IsNullOrWhiteSpace(contactName) ? "-" : contactName,
                EmailAddresses = applicationTask.Result.EmailAddresses,
                WebsiteUrl = string.IsNullOrEmpty(applicationTask.Result.BusinessWebsite) ? "-" : applicationTask.Result.BusinessWebsite,
            };
        }

        public async Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest)
        {
            var getContactDetailsResult = await _opportunitiesService.GetContactDetails(contactDetailsRequest.AccountId, contactDetailsRequest.PledgeId);

            if (getContactDetailsResult == null)
            {
                return null;
            }

            var opportunitySummaryViewModel = GetOpportunitySummaryViewModel(
                getContactDetailsResult.Sectors,
                getContactDetailsResult.JobRoles,
                getContactDetailsResult.Levels,
                getContactDetailsResult.AllSectors,
                getContactDetailsResult.AllJobRoles,
                getContactDetailsResult.AllLevels,
                getContactDetailsResult.Amount,
                getContactDetailsResult.IsNamePublic,
                getContactDetailsResult.DasAccountName,
                contactDetailsRequest.EncodedPledgeId);

            var cacheItem = await RetrieveCacheItem(contactDetailsRequest.CacheKey);

            var additionalEmailAddresses = cacheItem.EmailAddresses.Skip(1).ToList();

            var placeholders = Enumerable.Range(0, MaximumNumberAdditionalEmailAddresses - additionalEmailAddresses.Count())
                .Select(x => (string)null);
            
            additionalEmailAddresses.AddRange(placeholders);

            var viewModel = new ContactDetailsViewModel()
            {
                EncodedAccountId = contactDetailsRequest.EncodedAccountId,
                EncodedPledgeId = contactDetailsRequest.EncodedPledgeId,
                CacheKey = contactDetailsRequest.CacheKey,
                FirstName = cacheItem.FirstName,
                LastName = cacheItem.LastName,
                EmailAddress = cacheItem.EmailAddresses.FirstOrDefault(),
                AdditionalEmailAddresses = additionalEmailAddresses.ToArray(),
                BusinessWebsite = cacheItem.BusinessWebsite,
                DasAccountName = getContactDetailsResult.DasAccountName,
                OpportunitySummaryViewModel = opportunitySummaryViewModel,
            };

            return viewModel;
        }

        public async Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest)
        {
            var cacheItem = await RetrieveCacheItem(contactDetailsPostRequest.CacheKey);

            cacheItem.FirstName = contactDetailsPostRequest.FirstName;
            cacheItem.LastName = contactDetailsPostRequest.LastName;

            cacheItem.EmailAddresses.Clear();
            cacheItem.EmailAddresses.Add(contactDetailsPostRequest.EmailAddress);
            cacheItem.EmailAddresses.AddRange(contactDetailsPostRequest.AdditionalEmailAddresses.Where(x => !string.IsNullOrWhiteSpace(x)));

            cacheItem.BusinessWebsite = contactDetailsPostRequest.BusinessWebsite;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request)
        {
            var applicationTask = RetrieveCacheItem(request.CacheKey);
            var moreDetailsResponseTask = _opportunitiesService.GetMoreDetails(request.AccountId, request.PledgeId);

            await Task.WhenAll(applicationTask, moreDetailsResponseTask);

            return new MoreDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Details = applicationTask.Result.Details,
                OpportunitySummaryViewModel = GetOpportunitySummaryViewModel
                    (
                        moreDetailsResponseTask.Result.Opportunity.Sectors,
                        moreDetailsResponseTask.Result.Opportunity.JobRoles,
                        moreDetailsResponseTask.Result.Opportunity.Levels,
                        moreDetailsResponseTask.Result.Sectors,
                        moreDetailsResponseTask.Result.JobRoles,
                        moreDetailsResponseTask.Result.Levels,
                        moreDetailsResponseTask.Result.Opportunity.Amount,
                        moreDetailsResponseTask.Result.Opportunity.IsNamePublic,
                        moreDetailsResponseTask.Result.Opportunity.DasAccountName,
                        request.EncodedPledgeId
                    )
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);
            var response = await _opportunitiesService.GetSector(request.AccountId, request.PledgeId);

            return new SectorViewModel
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Sectors = cacheItem.Sectors,
                SectorOptions = response.Sectors.ToList(),
                OpportunitySummaryViewModel = GetOpportunitySummaryViewModel
                    (
                        response.Opportunity.Sectors,
                        response.Opportunity.JobRoles,
                        response.Opportunity.Levels,
                        response.Sectors,
                        response.JobRoles,
                        response.Levels,
                        response.Opportunity.Amount,
                        response.Opportunity.IsNamePublic,
                        response.Opportunity.DasAccountName,
                        request.EncodedPledgeId
                    ),
                Postcode = cacheItem.Postcode
            };
        }

        public async Task UpdateCacheItem(MoreDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Details = request.Details;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(ApplicationDetailsPostRequest request, int amount)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.JobRole = request.SelectedStandardTitle;
            cacheItem.StandardId = request.SelectedStandardId;
            cacheItem.NumberOfApprentices = request.NumberOfApprentices.Value;
            cacheItem.StartDate = request.StartDate;
            cacheItem.HasTrainingProvider = request.HasTrainingProvider.Value;
            cacheItem.Amount = amount;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(SectorPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Sectors = request.Sectors;
            cacheItem.Postcode = request.Postcode.ToUpper();

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        private async Task<CreateApplicationCacheItem> RetrieveCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<CreateApplicationCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreateApplicationCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }

        public async Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request)
        {
            var applicationDetailsTask = _opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId);
            var applicationTask = RetrieveCacheItem(request.CacheKey);

            await Task.WhenAll(applicationDetailsTask, applicationTask);

            var application = applicationTask.Result;
            var applicationDetails = applicationDetailsTask.Result;

            return new ApplicationDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                JobRole = application.JobRole,
                NumberOfApprentices = application.NumberOfApprentices,
                Month = application.StartDate?.Month,
                Year = application.StartDate?.Year,
                HasTrainingProvider = application.HasTrainingProvider,
                OpportunitySummaryViewModel = GetOpportunitySummaryViewModel
                    (
                        applicationDetails.Opportunity.Sectors,
                        applicationDetails.Opportunity.JobRoles,
                        applicationDetails.Opportunity.Levels,
                        applicationDetails.Sectors,
                        applicationDetails.JobRoles,
                        applicationDetails.Levels,
                        applicationDetails.Opportunity.Amount,
                        applicationDetails.Opportunity.IsNamePublic,
                        applicationDetails.Opportunity.DasAccountName,
                        request.EncodedPledgeId
                    ),
                MinYear = DateTime.Now.Year,
                MaxYear = DateTime.Now.FinancialYearEnd().Year,
                SelectStandardViewModel = new SelectStandardViewModel
                {
                    Standards = applicationDetails.Standards.Select(app => new StandardsListItemViewModel
                    {
                        Id = app.StandardUId,
                        LarsCode = app.LarsCode,
                        Level = app.Level,
                        Title = app.Title,
                        Selected = !string.IsNullOrEmpty(application.StandardId) && (app.StandardUId == application.StandardId) ? "selected": null
                    })
                }
            };
        }

        public async Task<ApplicationRequest> PostApplicationViewModel(ApplicationDetailsPostRequest request)
        {
            var applicationDetails = await _opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId);

            var amount = (await GetFundingEstimate(new GetFundingEstimateRequest
            {
                StartDate = request.StartDate.Value,
                SelectedStandardId = request.SelectedStandardId,
                NumberOfApprentices = request.NumberOfApprentices.Value,
                PledgeId = request.PledgeId
            }, applicationDetails)).Amount;

            request.SelectedStandardTitle = applicationDetails.Standards
                    .FirstOrDefault(standard => standard.StandardUId == request.SelectedStandardId)?.Title;

            await UpdateCacheItem(request, amount);

            return new ApplicationRequest
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                CacheKey = request.CacheKey
            };
        }

        public async Task<GetFundingEstimateViewModel> GetFundingEstimate(GetFundingEstimateRequest request, GetApplicationDetailsResponse applicationDetails = null)
        {
            applicationDetails ??= await _opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId);

            var amount = applicationDetails.Standards.Single()
                .ApprenticeshipFunding.GetEffectiveFundingLine(request.StartDate)
                .CalcFundingForDate(request.NumberOfApprentices, request.StartDate);

            return new GetFundingEstimateViewModel()
            {
                Amount = amount,
                HasEnoughFunding = applicationDetails.Opportunity.RemainingAmount >= amount
            };
        }

    }
}