using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService
{
    public interface IAccountsService
    {
        Task<int> GetRemainingTransferAllowance(string encodedAccountId);
    }
}
