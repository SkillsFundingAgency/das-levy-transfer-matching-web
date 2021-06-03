using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService
{
    public interface IAccountsService
    {
        public Task<int> GetRemainingTransferAllowance(string encodedAccountId);
    }
}
