using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers
{
    public interface IAccountUserService
    {
        Task<EmployerUserAccounts> GetUserAccounts(string email, string userId);
    }
}