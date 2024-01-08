using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Filters;

public class GoogleAnalyticsFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not Controller controller)
        {
            return;
        }

        controller.ViewBag.GaData = PopulateGaData(context);

        base.OnActionExecuting(context);
    }

    private static GaData PopulateGaData(ActionExecutingContext context)
    {
        string hashedAccountId = null;

        var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimIdentifierConfiguration.Id))?.Value;

        if (context.RouteData.Values.TryGetValue("AccountHashedId", out var accountHashedId))
        {
            hashedAccountId = accountHashedId.ToString();
        }

        return new GaData
        {
            UserId = userId,
            Acc = hashedAccountId
        };
    }
}