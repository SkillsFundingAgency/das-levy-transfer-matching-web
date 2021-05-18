using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.CosmosDb
{
    public class DocumentClientFactory : IDocumentClientFactory
    {
        private readonly Lazy<IDocumentClient> _documentClient;

        public DocumentClientFactory(CosmosDbConfiguration configuration)
        {
            _documentClient = new Lazy<IDocumentClient>(() =>
                new DocumentClient(
                    new Uri(configuration.Uri),
                    configuration.AuthKey,
                    new ConnectionPolicy
                    {
                        RetryOptions =
                        {
                            MaxRetryAttemptsOnThrottledRequests = 2,
                            MaxRetryWaitTimeInSeconds = 2
                        }
                    })
            );
        }

        public IDocumentClient CreateDocumentClient()
        {
            return _documentClient.Value;
        }
    }
}
