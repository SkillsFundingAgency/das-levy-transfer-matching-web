using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.Encoding;

namespace SFA.DAS.LevyTransferMatching.Web.ModelBinders;

public class AutoDecodeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.IsComplexType || (context.Metadata.ModelType != typeof(long) && context.Metadata.ModelType != typeof(int)))
        {
            return null;
        }
        
        var encodingService = context.Services.GetRequiredService<IEncodingService>();
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        var fallBackBinder = new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);

        return new AutoDecodeModelBinder(fallBackBinder, encodingService);
    }
}