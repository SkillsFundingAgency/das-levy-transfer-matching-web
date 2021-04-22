using System.Text.Json.Serialization;

namespace SFA.DAS.LevyTransferMatching.Domain.ApiClient
{
    public interface IGetAllApiRequest : IBaseApiRequest
    {
        [JsonIgnore]
        string GetAllUrl { get; }
    }
}
