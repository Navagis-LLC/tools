using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http.Filters;

namespace NavagisInternalTool.Credentials
{
    public class AdminRequiresAuthentication : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["isLogedIn"] != "Yes")
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