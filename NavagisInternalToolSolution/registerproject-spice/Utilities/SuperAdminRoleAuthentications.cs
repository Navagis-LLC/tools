using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace RegisterProject_Spice.Utilities
{
    public class SuperAdminRoleAuthentications : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isSuperAdmin = filterContext.HttpContext.Session.GetString("isSuperAdmin");

            if (isSuperAdmin != "yes")
                RedirectToLogin(filterContext);
        }

        private void RedirectToLogin(ActionExecutingContext filterContext)
        {
            var redirectTarget = new RouteValueDictionary
            {
                {"action", "insufficientPrivileges"},
                {"controller", "CustomMessages"}
            };

            filterContext.Result = new RedirectToRouteResult(redirectTarget);
        }
    }
}
