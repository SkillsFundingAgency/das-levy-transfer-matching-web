﻿using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task<long> PostPledge(PledgeDto pledgeDto, long accountId);
        Task<GetPledgesResponse> GetPledges(long accountId);
        Task<GetCreateResponse> GetCreate(long accountId);
        Task<GetAmountResponse> GetAmount(string encodedAccountId);
        Task<GetSectorResponse> GetSector(long accountId);
        Task<GetJobRoleResponse> GetJobRole(long accountId);
        Task<GetLevelResponse> GetLevel(long accountId);
    }
}