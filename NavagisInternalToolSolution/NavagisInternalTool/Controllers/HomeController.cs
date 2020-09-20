using System.Web.Mvc;
using System.Threading;
using Google.Apis.Auth.OAuth2.Mvc;
using NavagisInternalTool.Credentials;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace NavagisInternalTool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Connect(CancellationToken cancellationToken)
        {
            var _authorizationCodeApp = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (_authorizationCodeApp.Credential == null)
                return new RedirectResult(_authorizationCodeApp.RedirectUri);

            return RedirectToAction("MyProjects", "GoogleProjects");
        }
    }
}