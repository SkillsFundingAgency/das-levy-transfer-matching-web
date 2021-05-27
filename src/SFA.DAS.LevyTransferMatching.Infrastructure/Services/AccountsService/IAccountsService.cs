using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService
{
    public interface IAccountsService
    {
        public Task<string> GetTransferAllowance(string encodedAccountId);
    }
}
