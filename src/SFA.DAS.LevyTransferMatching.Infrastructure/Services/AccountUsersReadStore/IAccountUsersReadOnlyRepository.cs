using SFA.DAS.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore
{
    public interface IAccountUsersReadOnlyRepository : IReadOnlyDocumentRepository<AccountUser>
    {
    }
}
