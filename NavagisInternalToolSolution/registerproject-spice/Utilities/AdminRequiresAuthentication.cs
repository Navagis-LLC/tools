using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace RegisterProject_Spice.Utilities
{
    public class AdminRequiresAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isLogedIn = filterContext.HttpContext.Session.GetString("isLogedIn");

            if (isLogedIn != "Yes")
                RedirectToLogin(filterContext);
        }

        private void RedirectToLogin(ActionExecutingContext filterContext)
        {
            var redirectTarget = new RouteValueDictionary
            {
                {"action", "login"},
                {"controller", "Admin"}
            };

            filterContext.Result = new RedirectToRouteResult(redirectTarget);
        }
    }
}
