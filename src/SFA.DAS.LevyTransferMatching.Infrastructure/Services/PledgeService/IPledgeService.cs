using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task<long> PostPledge(PledgeDto pledgeDto, long accountId);
        Task<GetCreateResponse> GetCreate(long accountId);
    }
}
