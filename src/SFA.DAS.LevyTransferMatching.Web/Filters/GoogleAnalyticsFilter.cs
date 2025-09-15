using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsers.Types;
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
        string levyFlag = null;

        var userId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimIdentifierConfiguration.Id))?.Value;

        if (context.RouteData.Values.TryGetValue("encodedAccountId", out var accountHashedId))
        {
            hashedAccountId = accountHashedId.ToString();

            var accountsJson = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier))?.Value;
            if (accountsJson is not null)
            {
                var accounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerIdentifier>>(accountsJson);
                levyFlag = accounts.TryGetValue(hashedAccountId, out var employer) ? employer.ApprenticeshipEmployerType.ToString() : null;
            }
        }

        return new GaData
        {
            UserId = userId,
            Acc = hashedAccountId,
            LevyFlag = levyFlag
        };
    }
}