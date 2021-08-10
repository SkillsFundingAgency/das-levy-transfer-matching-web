using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserAccountDto>> GetLoggedInUserAccounts();
        bool IsUserChangeAuthorized();
    }
}