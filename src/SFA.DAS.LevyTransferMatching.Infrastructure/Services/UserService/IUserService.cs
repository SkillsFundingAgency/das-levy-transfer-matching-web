using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService
{
    public interface IUserService
    {
        string GetUserId();

        string GetUserDisplayName();

        bool IsUserChangeAuthorized();

        IEnumerable<long> GetUserAccountIds();
    }
}