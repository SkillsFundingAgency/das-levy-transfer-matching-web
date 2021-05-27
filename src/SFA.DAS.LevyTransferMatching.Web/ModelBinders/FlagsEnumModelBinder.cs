using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.LevyTransferMatching.Web.ModelBinders
{
    public class FlagsEnumModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var modelType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;

            if (!modelType.IsEnum || !Attribute.IsDefined(modelType, typeof(FlagsAttribute)))
            {
                return Task.CompletedTask;
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                var zeroValue = Enum.Parse(modelType, "0");
                valueProviderResult = new ValueProviderResult(zeroValue.ToString());
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            if (Enum.TryParse(modelType, valueProviderResult.Values, true, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }

            return Task.CompletedTask;
        }
    }

}
