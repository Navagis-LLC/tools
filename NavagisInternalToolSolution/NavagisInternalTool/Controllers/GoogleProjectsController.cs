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

using NavagisInternalTool.Credentials;
using NavagisInternalTool.Models;


namespace NavagisInternalTool.Controllers
{
    public class GoogleProjectsController : Controller
    {
        private string _billingAccountName;

        public GoogleProjectsController()
        {
            var _setting = new Setting();
            _billingAccountName = _setting.BillingAccountName;
        }

        public async Task<ActionResult> MyProjects(CancellationToken cancellationToken)
        {
            var _googleConnect = new ConnectedServices(this, cancellationToken);
            var _isAutorize = await _googleConnect.Authorize();

            if(_isAutorize==false)
                return RedirectToAction("Index", "Home");
            
            var projects = RetriveProjects(_googleConnect);
            var linkedProjects = RetriveProjectsInBilling(_googleConnect);

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

        /**
        * A user must have "Billing Viewer" role set in the console billing.  
        */
        private List<ProjectsInBilling> RetriveProjectsInBilling(ConnectedServices _googleConnect)
        {
            var linkedProjects = new List<ProjectsInBilling>();
            var service = _googleConnect.GetCloudbillingService();

            BillingAccountsResource.ProjectsResource.ListRequest
                listRequest = service.BillingAccounts.Projects.List(_billingAccountName);
        
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