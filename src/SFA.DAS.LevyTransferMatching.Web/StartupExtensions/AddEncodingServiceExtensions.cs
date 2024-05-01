using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddEncodingServiceExtensions
{
    public static void AddEncodingService(this IServiceCollection services)
    {
        services.AddSingleton<IEncodingService, EncodingService>();
    }
}