using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    public abstract class LoggedInController : Controller
    {
        public override ViewResult View(object model)
        {
            ViewBag.HideNav = false;

            return base.View(model);
        }
    }
}