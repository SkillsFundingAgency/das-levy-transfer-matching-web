using Microsoft.Azure.Documents;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb
{
    public interface IDocumentClientFactory
    {
        IDocumentClient CreateDocumentClient();
    }
}
