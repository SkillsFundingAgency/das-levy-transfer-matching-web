using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService
{
    public interface IPledgeService
    {
        Task PostPledge(PledgeDto pledgeDto, long accountId);
    }
}
