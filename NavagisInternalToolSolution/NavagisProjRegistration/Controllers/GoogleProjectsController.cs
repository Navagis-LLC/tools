using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System;

using Google.Apis.Cloudbilling.v1;
using DataCloudResourceManager = Google.Apis.CloudResourceManager.v1.Data;
using ProjectsResource = Google.Apis.CloudResourceManager.v1.ProjectsResource;
using DataCloudbilling = Google.Apis.Cloudbilling.v1.Data;

using NavagisProjRegistration.Credentials;
using NavagisProjRegistration.Models;


namespace NavagisProjRegistration.Controllers
{
    public class GoogleProjectsController : Controller
    {
        private ApplicationDBContext db;
        private int _settingRecordID;
        

        public GoogleProjectsController()
        {
            db = new ApplicationDBContext();
            _settingRecordID = Convert.ToInt32(WebConfigurationManager.AppSettings["DBDefaultSettingID"]);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public ActionResult Testing()
        {
            throw new Exception();
            return Content("this is a test.");
        }

        public ActionResult EmailNoBillingAccount(string email)
        {
            ViewBag.email = email; 
            return View();
        }
        

        public async Task<ActionResult> MyProjects(CancellationToken cancellationToken)
        {
            var _googleConnect = new ConnectedServices(this, cancellationToken);
            var _isAutorize = await _googleConnect.Authorize();

          
            if (_isAutorize==false)
                return RedirectToAction("Index", "Home");

            // get user Email
            var oauth2Service = _googleConnect.GetOauth2Service();
            var userInfo = oauth2Service.Userinfo.Get().Execute();

            Client client = db.Clients.SingleOrDefault(c => c.Email == userInfo.Email);
            
            if (client==null)
            {
                // logout User - todo - below seems not working.
                var AuthResult = _googleConnect.GetAuthResult();
                AuthResult.Credential = null;
                Session.Abandon();
                //-------------------------//

                // Inform the user - his email has no billing account assigned.
                return RedirectToAction("EmailNoBillingAccount", "GoogleProjects", new { email = userInfo.Email });
            }
                
            BillingAccount billingAccount = db.BillingAccounts.SingleOrDefault(b => b.Id == client.BillingAccountId);
            var billingNumber = "billingAccounts/" + billingAccount.BillingAccountName;

    
            var projects = RetriveProjects(_googleConnect);
            var linkedProjects = RetriveProjectsInBilling(_googleConnect, billingNumber);

            foreach(Project project in projects)
            {
                var _linkProject = linkedProjects.SingleOrDefault(p => p.ProjectId == project.ProjectId);

                project.IsLinked = false;
                project.IsLinkedText = "No";

                if (_linkProject != null)
                {
                    project.IsLinked = true;
                    project.IsLinkedText = "Yes";
                    project.TableTrClass = "success";
                    project.TableRadioButton = "disabled";
                }
            }

            return View(projects);
        }

        private List<Project> RetriveProjects(ConnectedServices _googleConnect)
        {
            var projects = new List<Project>();
            var service = _googleConnect.GetCloudResourceManagerService();
            ProjectsResource.ListRequest request = service.Projects.List();

            DataCloudResourceManager.ListProjectsResponse response;
            do
            {
                response = request.Execute();
                if (response.Projects == null)
                    continue;

                foreach (DataCloudResourceManager.Project project in response.Projects)
                {
                    projects.Add(
                        new Project
                        {
                            ProjectId = project.ProjectId,
                            ProjectNumber = (long)project.ProjectNumber,
                            Name = project.Name,
                            LifecycleState = project.LifecycleState
                        }
                    );
                }
                request.PageToken = response.NextPageToken;
            }
            while (response.NextPageToken != null);
            return projects;
        }

        private List<ProjectsInBilling> RetriveProjectsInBilling(ConnectedServices _googleConnect, string billingNumber)
        {
            var linkedProjects = new List<ProjectsInBilling>();
            var service = _googleConnect.GetCloudbillingService();

            BillingAccountsResource.ProjectsResource.ListRequest
            listRequest = service.BillingAccounts.Projects.List(billingNumber);
        
            DataCloudbilling.ListProjectBillingInfoResponse response;

            try
            {
                do
                {
                    response = listRequest.Execute();
                    if (response.ProjectBillingInfo == null)
                        continue;

                    foreach (DataCloudbilling.ProjectBillingInfo billingInfo in response.ProjectBillingInfo)
                    {
                        linkedProjects.Add(
                            new ProjectsInBilling
                            {
                                BillingAccountName = billingInfo.BillingAccountName,
                                BillingEnabled = (bool)billingInfo.BillingEnabled,
                                Name = billingInfo.Name,
                                ProjectId = billingInfo.ProjectId
                            }
                        );
                    }
                }
                while (response.NextPageToken != null);
            }
            catch (Exception)
            {}
            
            return linkedProjects;
        }
    }
}