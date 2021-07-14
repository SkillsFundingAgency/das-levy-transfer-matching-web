﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IAccountsService _accountsService;
        private readonly IPledgeService _pledgeService;
        private readonly ITagService _tagService;
        private readonly IEncodingService _encodingService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService, IAccountsService accountsService, IPledgeService pledgeService, ITagService tagService, IEncodingService encodingService)
        {
            _cacheStorageService = cacheStorageService;
            _accountsService = accountsService;
            _pledgeService = pledgeService;
            _tagService = tagService;
            _encodingService = encodingService;
        }

        public InformViewModel GetInformViewModel(string encodedAccountId)
        {
            return new InformViewModel
            {
                EncodedAccountId = encodedAccountId,
                CacheKey = Guid.NewGuid()
            };
        }

        public async Task<CreateViewModel> GetCreateViewModel(CreateRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var dataTask = _pledgeService.GetCreate(request.AccountId);
            await Task.WhenAll(cacheItemTask, dataTask);

            var cacheItem = cacheItemTask.Result;

            return new CreateViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount,
                IsNamePublic = cacheItem.IsNamePublic,
                Sectors = cacheItem.Sectors,
                JobRoles = cacheItem.JobRoles,
                Levels = cacheItem.Levels,
                LevelOptions = dataTask.Result.Levels.ToList(),
                SectorOptions = dataTask.Result.Sectors.ToList(),
                JobRoleOptions = dataTask.Result.JobRoles.ToList()
            };
        }

        public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var accountDataTask = _pledgeService.GetAmount(request.EncodedAccountId);

            await Task.WhenAll(cacheItemTask, accountDataTask);
            var cacheItem = cacheItemTask.Result;
            var accountData = accountDataTask.Result;

            return new AmountViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount.ToString(),
                RemainingTransferAllowance = accountData.RemainingTransferAllowance.ToString("N0"),
                IsNamePublic = cacheItem.IsNamePublic,
                DasAccountName = accountData.DasAccountName
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var sectorsTask = _tagService.GetSectors();

            await Task.WhenAll(cacheItemTask, sectorsTask);

            return new SectorViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Sectors = cacheItemTask.Result.Sectors,
                SectorOptions = sectorsTask.Result
            };
        }

        public async Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var jobRolesTask = _tagService.GetJobRoles();

            await Task.WhenAll(cacheItemTask, jobRolesTask);

            return new JobRoleViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                JobRoles = cacheItemTask.Result.JobRoles,
                JobRoleOptions = jobRolesTask.Result
            };
        }

        public async Task<string> SubmitPledge(CreatePostRequest request)
        {
            var cacheItem = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(request.CacheKey.ToString());

            ValidateCreatePledgeCacheItem(cacheItem);

            var pledgeDto = new PledgeDto
            {
                Amount = cacheItem.Amount.Value,
                IsNamePublic = cacheItem.IsNamePublic.Value,
                DasAccountName = cacheItem.DasAccountName,
                Sectors = cacheItem.Sectors ?? new List<string>(),
                JobRoles = cacheItem.JobRoles ?? new List<string>(),
                Levels = cacheItem.Levels ?? new List<string>()
            };

            var pledgeId = await _pledgeService.PostPledge(pledgeDto, request.AccountId);
            await _cacheStorageService.DeleteFromCache(request.CacheKey.ToString());

            return _encodingService.Encode(pledgeId, EncodingType.PledgeId);
        }

        public async Task UpdateCacheItem(AmountPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Amount = Int32.Parse(request.Amount);
            cacheItem.IsNamePublic = request.IsNamePublic.Value;
            cacheItem.DasAccountName = request.DasAccountName;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(SectorPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Sectors = request.Sectors;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(JobRolePostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.JobRoles = request.JobRoles;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(LevelPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Levels = request.Levels;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task<LevelViewModel> GetLevelViewModel(LevelRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var levelsTask = _tagService.GetLevels();

            await Task.WhenAll(cacheItemTask, levelsTask);

            return new LevelViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Levels = cacheItemTask.Result.Levels,
                LevelOptions = levelsTask.Result
            };
        }

        private async Task<CreatePledgeCacheItem> RetrievePledgeCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreatePledgeCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }

        private void ValidateCreatePledgeCacheItem(CreatePledgeCacheItem cacheItem)
        {
            if (cacheItem == null)
            {
                throw new InvalidOperationException("Unable to submit pledge due to cache expiry");
            }
            if (!cacheItem.Amount.HasValue)
            {
                throw new InvalidOperationException("Unable to submit pledge due to null cache value for pledge Amount");
            }
            if (!cacheItem.IsNamePublic.HasValue)
            {
                throw new InvalidOperationException("Unable to submit pledge due to null cache value for pledge IsNamePublic");
            }
        }
    }
}
