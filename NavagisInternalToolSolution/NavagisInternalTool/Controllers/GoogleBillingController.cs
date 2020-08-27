using System.Web.Mvc;
using System.Threading;
using System.Linq;

using Google.Apis.Cloudbilling.v1;
using Data = Google.Apis.Cloudbilling.v1.Data;

using NavagisInternalTool.Credentials;
using NavagisInternalTool.Models;
using System.Web.Configuration;
using System;
using System.Threading.Tasks;

namespace NavagisInternalTool.Controllers
{
    public class GoogleBillingController : Controller
    {
        private string _billingAccountName;

        public GoogleBillingController()
        {
            var _setting = new Setting();
            _billingAccountName = _setting.BillingAccountName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LinkProject(CancellationToken cancellationToken, string CategoryId)
        {
            var _googleConnect = new ConnectedServices(this, cancellationToken);
            var _isAutorize = await _googleConnect.Authorize();

            if (_isAutorize == false)
                return RedirectToAction("Index", "Home");

            var _cloudbillingService = _googleConnect.GetCloudbillingService();
            var project = "projects/"+ CategoryId;

            try
            {
                Data.ProjectBillingInfo projectBillingInfo = new Data.ProjectBillingInfo();
                projectBillingInfo.BillingAccountName = _billingAccountName;

                ProjectsResource.UpdateBillingInfoRequest updateBillingInfoRequest =
                    _cloudbillingService.Projects.UpdateBillingInfo(projectBillingInfo, project);

                Data.ProjectBillingInfo response = updateBillingInfoRequest.Execute();
                Session["ErrMessage"] = "";
            }
            catch (Exception)
            {
                Session["ErrMessage"] = "There is a permission issue in linking the selected project to Navagis. Please conctact us.";
            }
            

            return RedirectToAction("MyProjects", "GoogleProjects"); 
        }
    }
}