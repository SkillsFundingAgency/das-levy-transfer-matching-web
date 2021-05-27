using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace SFA.DAS.LevyTransferMatching.Web.ModelBinders
{
    public class FlagsEnumModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType.IsEnum && context.Metadata.ModelType.IsDefined(typeof(FlagsAttribute), true))
            {
                return new BinderTypeModelBinder(typeof(FlagsEnumModelBinder));
            }

            return null;
        }
    }
}
