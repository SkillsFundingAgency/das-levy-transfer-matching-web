using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountsService
{
    public interface IAccountsService
    {
        Task<AccountDto> GetAccountDetail(string encodedAccountId);
    }
}
