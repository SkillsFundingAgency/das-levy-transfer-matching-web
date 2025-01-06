using System.Text.Json.Serialization;

namespace SFA.DAS.LevyTransferMatching.Domain.ApiClient;

public interface IBaseApiRequest
{
    [JsonIgnore]
    string BaseUrl { get; }
}