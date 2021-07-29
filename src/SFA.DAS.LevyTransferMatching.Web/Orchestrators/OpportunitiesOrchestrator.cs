using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
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
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using FluentValidation;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private const int MaximumNumberAdditionalEmailAddresses = 4;

        private readonly ICacheStorageService _cacheStorageService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly ITagService _tagService;
        private readonly IEncodingService _encodingService;
        private readonly IUserService _userService;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService, IUserService userService, IEncodingService encodingService, ICacheStorageService cacheStorageService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _tagService = tagService;
            _encodingService = encodingService;
            _tagService = tagService;
            _userService = userService;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<DetailViewModel> GetDetailViewModel(int pledgeId)
        {
            var opportunityDto = await _opportunitiesService.GetOpportunity(pledgeId);

            if (opportunityDto == null)
            {
                return null;
            }

            var encodedPledgeId = _encodingService.Encode(opportunityDto.Id, EncodingType.PledgeId);

            var opportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, encodedPledgeId);

            return new DetailViewModel()
            {
                EmployerName = opportunityDto.DasAccountName,
                EncodedPledgeId = encodedPledgeId,
                IsNamePublic = opportunityDto.IsNamePublic,
                OpportunitySummaryView = opportunitySummaryViewModel,
            };
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = _opportunitiesService.GetAllOpportunities();
            var levelsTask = _tagService.GetLevels();
            var sectorsTask = _tagService.GetSectors();
            var jobRolesTask = _tagService.GetJobRoles();

            await Task.WhenAll(opportunitiesDto, levelsTask, sectorsTask, jobRolesTask);

            List<Opportunity> opportunities = opportunitiesDto.Result
                .Select(x => new Opportunity
                {
                    Amount = x.Amount,
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                    Sectors = x.Sectors.ToReferenceDataDescriptionList(sectorsTask.Result),
                    JobRoles = x.JobRoles.ToReferenceDataDescriptionList(jobRolesTask.Result),
                    Levels = x.Levels.ToReferenceDataDescriptionList(levelsTask.Result, descriptionSource: y => y.ShortDescription),
                    Locations = x.Locations
                }).ToList();

            return new IndexViewModel 
            { 
                Opportunities = opportunities
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

        [Obsolete("To eventually be replaced with the other method of the same name - please use other overload.")]
        public async Task<OpportunitySummaryViewModel> GetOpportunitySummaryViewModel(OpportunityDto opportunityDto, string encodedPledgeId)
        {
            // Pull back the tags, and use the descriptions to build the lists.
            var sectorReferenceDataItems = await _tagService.GetSectors();
            string sectorList = opportunityDto.Sectors.ToReferenceDataDescriptionList(sectorReferenceDataItems);

            var jobRoleReferenceDataItems = await _tagService.GetJobRoles();
            string jobRoleList = opportunityDto.JobRoles.ToReferenceDataDescriptionList(jobRoleReferenceDataItems);

            var levelReferenceDataItems = await _tagService.GetLevels();
            string levelList = opportunityDto.Levels.ToReferenceDataDescriptionList(levelReferenceDataItems, descriptionSource: x => x.ShortDescription);

            DateTime dateTime = _dateTimeService.UtcNow;

            return new OpportunitySummaryViewModel()
            {
                Amount = opportunityDto.Amount,
                Description = GenerateDescription(opportunityDto, encodedPledgeId),
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                YearDescription = dateTime.ToTaxYearDescription(),
            };
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
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity(request.PledgeId);
            var sectorOptions = await _tagService.GetSectors();

            var contactName = $"{application.FirstName} {application.LastName}";

            return new ApplyViewModel
            {
                CacheKey = application.Key,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedAccountId = request.EncodedAccountId,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
                JobRole = application.JobRole ?? "-",
                NumberOfApprentices = application.NumberOfApprentices.HasValue ? application.NumberOfApprentices.Value.ToString() : "-",
                StartBy = application.StartDate.HasValue ? application.StartDate.Value.ToShortDisplayString() : "-",
                HaveTrainingProvider = application.HasTrainingProvider.ToApplyViewString(),
                Sectors = application.Sectors?.ToList(),
                SectorOptions = sectorOptions,
                Location = application.Postcode ?? "-",
                MoreDetail = application.Details ?? "-",
                ContactName = string.IsNullOrWhiteSpace(contactName) ? "-" : contactName,
                EmailAddresses = application.EmailAddresses,
                WebsiteUrl = string.IsNullOrEmpty(application.BusinessWebsite) ? "-" : application.BusinessWebsite,
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

        private string GenerateDescription(OpportunityDto opportunityDto, string encodedPledgeId)
        {
            return opportunityDto.IsNamePublic ? $"{opportunityDto.DasAccountName} ({encodedPledgeId})" : "A levy-paying business wants to fund apprenticeship training in:";
        }

        public async Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request)
        {
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity((int)_encodingService.Decode(request.EncodedPledgeId, EncodingType.PledgeId));

            return new MoreDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Details = application.Details,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
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
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(response.Opportunity, request.EncodedPledgeId),
                Postcode = cacheItem.Postcode
            };
        }

        public async Task UpdateCacheItem(MoreDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Details = request.Details;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(ApplicationDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.JobRole = request.SelectedStandardTitle;
            cacheItem.StandardId = request.SelectedStandardId;
            cacheItem.NumberOfApprentices = request.NumberOfApprentices.Value;
            cacheItem.StartDate = request.StartDate;
            cacheItem.HasTrainingProvider = request.HasTrainingProvider.Value;

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
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(applicationDetails.Opportunity, request.EncodedPledgeId),
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
            var applicationDetails = await _opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId);

            request.SelectedStandardTitle = applicationDetails.Standards
                .FirstOrDefault(standard => standard.StandardUId == request.SelectedStandardId)?.Title;

            await UpdateCacheItem(request);

            return new ApplicationRequest
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                CacheKey = request.CacheKey
            };
        }
    }
}