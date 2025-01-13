using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.ModelBinders;

public class AutoDecodeModelBinder(IModelBinder fallbackBinder, IEncodingService encodingService)
    : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var attribute = GetAutoDecodeAttribute(bindingContext);

        if (attribute == null)
        {
            return fallbackBinder.BindModelAsync(bindingContext);
        }

        try
        {
            var encodedValue = bindingContext.ValueProvider.GetValue(attribute.Source).FirstValue;

            if (GetTargetType(bindingContext) == typeof(long))
            {
                var decodedValue = encodingService.Decode(encodedValue, attribute.EncodingType);
                bindingContext.Result = ModelBindingResult.Success(decodedValue);
            }
            else if (GetTargetType(bindingContext) == typeof(int))
            {
                var decodedValue = (int)encodingService.Decode(encodedValue, attribute.EncodingType);
                bindingContext.Result = ModelBindingResult.Success(decodedValue);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
        catch
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    private static AutoDecodeAttribute GetAutoDecodeAttribute(ModelBindingContext context)
    {
        var container = context.ModelMetadata.ContainerType;
        if (container == null) return null;

        var propertyType = container.GetProperty(context.ModelMetadata.PropertyName);
        var attributes = propertyType.GetCustomAttributes(true);
        var attribute = (AutoDecodeAttribute)attributes.FirstOrDefault(a => a.GetType().IsEquivalentTo(typeof(AutoDecodeAttribute)));

        return attribute;
    }

    private static Type GetTargetType(ModelBindingContext context)
    {
        var container = context.ModelMetadata.ContainerType;
        if (container == null)
        {
            return null;
        }
        
        var property = container.GetProperty(context.ModelMetadata.PropertyName);
        return property?.PropertyType;
    }
}