using Microsoft.Azure.Documents;
using SFA.DAS.CosmosDb;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore
{
    public class AccountUsersReadOnlyRepository : DocumentRepository<AccountUser>, IAccountUsersReadOnlyRepository
    {
        public AccountUsersReadOnlyRepository(IDocumentClient documentClient)
            : base(documentClient, DocumentSettings.DatabaseName, DocumentSettings.AccountUsersCollectionName)
        {
        }

        public AccountUsersReadOnlyRepository(IDocumentClientFactory documentClientFactory)
            : base(documentClientFactory.CreateDocumentClient(), DocumentSettings.DatabaseName, DocumentSettings.AccountUsersCollectionName)
        {
        }
    }
}
