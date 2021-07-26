using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.LevyTransferMatching.Web.FeatureToggles
{
    public class DisabledActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
