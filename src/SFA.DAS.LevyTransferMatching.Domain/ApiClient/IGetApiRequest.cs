using System.Text.Json.Serialization;

namespace SFA.DAS.LevyTransferMatching.Domain.ApiClient
{
    public interface IGetApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetUrl { get; }
    }
}
