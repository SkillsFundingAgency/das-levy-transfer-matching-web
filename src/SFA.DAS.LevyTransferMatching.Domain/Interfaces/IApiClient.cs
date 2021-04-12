using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Domain.Interfaces
{
    public interface IApiClient
    {
        Task Ping();
        Task<TResponse> Get<TResponse>(object request);
        Task<TResponse> Post<TResponse, TPostData>(object request);
    }
}
