﻿using System;
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
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
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
                    Sectors = sectorsTask.Result.Where(y => x.Sectors.Contains(y.Id)).Select(y => y.Description).ToList(),
                    JobRoles = jobRolesTask.Result.Where(y => x.JobRoles.Contains(y.Id)).Select(y => y.Description).ToList(),
                    Levels = levelsTask.Result.Where(y => x.Levels.Contains(y.Id)).Select(y => y.ShortDescription).ToList(),
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

        public async Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request)
        {
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity(request.PledgeId);

            return new ApplyViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
                JobRole = "-",
                NumberOfApprentices = "-",
                StartBy = "-",
                HaveTrainingProvider = "-",
                Sectors = "-",
                Locations = "-",
                MoreDetail = "-",
                ContactName = "-",
                EmailAddress = "-",
                WebsiteUrl = "-"
            };
        }

        public async Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest)
        {
            var opportunityDto = await _opportunitiesService.GetOpportunity(contactDetailsRequest.PledgeId);

            if (opportunityDto == null)
            {
                return null;
            }

            var encodedPledgeId = _encodingService.Encode(opportunityDto.Id, EncodingType.PledgeId);

            var opportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, encodedPledgeId);

            var cacheItem = await this.RetrieveCacheItem(contactDetailsRequest.CacheKey);

            IEnumerable<string> additionalEmailAddresses = cacheItem.AdditionalEmailAddresses;
            if (additionalEmailAddresses == null)
            {
                additionalEmailAddresses = Enumerable.Range(0, 4).Select(x => (string)null);
            }

            return new ContactDetailsViewModel()
            {
                EncodedAccountId = contactDetailsRequest.EncodedAccountId,
                EncodedPledgeId = contactDetailsRequest.EncodedPledgeId,
                FirstName = cacheItem.FirstName,
                LastName = cacheItem.LastName,
                EmailAddress = cacheItem.EmailAddress,
                AdditionalEmailAddresses = additionalEmailAddresses.ToArray(),
                BusinessWebsite = cacheItem.BusinessWebsite,
                DasAccountName = opportunityDto.DasAccountName,
                OpportunitySummaryViewModel = opportunitySummaryViewModel,
            };
        }

        public async Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest)
        {
            var cacheItem = await RetrieveCacheItem(contactDetailsPostRequest.CacheKey);

            cacheItem.FirstName = contactDetailsPostRequest.FirstName;
            cacheItem.LastName = contactDetailsPostRequest.LastName;
            cacheItem.EmailAddress = contactDetailsPostRequest.EmailAddress;
            cacheItem.AdditionalEmailAddresses = contactDetailsPostRequest.AdditionalEmailAddresses.ToList();
            cacheItem.BusinessWebsite = contactDetailsPostRequest.BusinessWebsite;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        private string GenerateDescription(OpportunityDto opportunityDto, string encodedPledgeId) => opportunityDto.IsNamePublic ? $"{opportunityDto.DasAccountName} ({encodedPledgeId})" : "A levy-paying business wants to fund apprenticeship training in:";

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
    }
}