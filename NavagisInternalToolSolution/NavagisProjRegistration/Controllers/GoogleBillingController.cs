using System.Web.Mvc;
using System.Threading;
using System.Linq;

using Google.Apis.Cloudbilling.v1;
using Data = Google.Apis.Cloudbilling.v1.Data;
using Data2 = Google.Apis.CloudResourceManager.v1.Data;

using NavagisProjRegistration.Credentials;
using NavagisProjRegistration.Models;
using System.Web.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NavagisProjRegistration.Controllers
{
    public class GoogleBillingController : Controller
    {
        private ApplicationDBContext db;
        private static int _settingRecordID = Convert.ToInt32(WebConfigurationManager.AppSettings["DBDefaultSettingID"]);
        private static Setting _setting = new ApplicationDBContext().Settings.SingleOrDefault(s => s.Id == _settingRecordID);


        public GoogleBillingController()
        {
            db = new ApplicationDBContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LinkProject(CancellationToken cancellationToken, string projectName)
        {
            var _googleConnect = new ConnectedServices(this, cancellationToken);
            var _isAutorize = await _googleConnect.Authorize();

            if (_isAutorize == false)
                return RedirectToAction("Index", "Home");

            var project = "projects/"+ projectName;
            var billingAccount = GetBillingAccountFromDB(_googleConnect);
            var billingAccountName = "billingAccounts/" + billingAccount.BillingAccountName;


            try
            {
                var serviceAccount = "serviceAccount:" + _setting.ServiceAccountEmail;
                CreateNewProjectIam(_googleConnect, projectName, "roles/owner", serviceAccount);
                ChangeBillingAccount( _googleConnect, billingAccountName, project);
                CreateNewBillingIam(_googleConnect, billingAccountName, "roles/billing.admin", "user:david@navagis.com");
                CreateNewBillingIam(_googleConnect, billingAccountName, "roles/billing.admin", "group:billing@navagis.com");

                Session["ErrMessage"] = "";
            }
            catch (Exception err)
            {

                Session["ErrMessage"] = "There is a permission issue in linking the selected project to Navagis. Please conctact us.";
                
                // Session["ErrMessage"] = err.Message + "<br>"+ err.StackTrace;
                /*
                 * to research
                 Google.Apis.Requests.RequestError The caller does not have permission [403] 
                 Errors [ Message[The caller does not have permission] Location[ - ] Reason[forbidden] Domain[global] ] 
                 */
            }

            return RedirectToAction("MyProjects", "GoogleProjects"); 
        }

        public BillingAccount GetBillingAccountFromDB(ConnectedServices _googleConnect)
        {
            var oauth2Service = _googleConnect.GetOauth2Service();
            var userInfo = oauth2Service.Userinfo.Get().Execute();
            Client client = db.Clients.SingleOrDefault(c => c.Email == userInfo.Email);
            BillingAccount billingAccount = db.BillingAccounts.SingleOrDefault(b => b.Id == client.BillingAccountId);
         
            return billingAccount;
        }

        public void ChangeBillingAccount(ConnectedServices _googleConnect, string billingAccountName, string project)
        {

            var _cloudbillingService = _googleConnect.GetCloudbillingService();
            Data.ProjectBillingInfo projectBillingInfo = new Data.ProjectBillingInfo();
            projectBillingInfo.BillingAccountName = billingAccountName;

            ProjectsResource.UpdateBillingInfoRequest updateBilling = new
                ProjectsResource.UpdateBillingInfoRequest(_cloudbillingService, projectBillingInfo, project);

            Data.ProjectBillingInfo response = updateBilling.Execute();

        }

        public void CreateNewProjectIam(ConnectedServices _googleConnect, string resource, string role, string member)
        {
            var managerService = _googleConnect.GetCloudResourceManagerService();
            Data2.GetIamPolicyRequest requestBody = new Data2.GetIamPolicyRequest();
            Google.Apis.CloudResourceManager.v1.ProjectsResource.GetIamPolicyRequest iamRequest = managerService.Projects.GetIamPolicy(requestBody, resource);
            Data2.Policy policyResponse = iamRequest.Execute();

            IList<Data2.Binding> bindings = policyResponse.Bindings;

            // Check if the bindings has the role specified already
            bool has = bindings.Any(binding => binding.Role == role);

            if (has)
            {
                // Get the first binding that has the specified role and add the member to it
                var binding = bindings.First(tempBinding => tempBinding.Role == role);

                if (!binding.Members.Contains(member))
                    bindings.First(tempBinding => tempBinding.Role == role).Members.Add(member);
                else  // The user specified is already added, exit function
                    return;

            }
            else // no binding exists for this role type
            {
                Data2.Binding binding = new Data2.Binding();
                binding.Role = role;
                binding.Members = new List<string>() { member };

                bindings.Add(binding);

            }

            // It seems we don't need to do this since bindings is already == policyResponse.Bindings
            policyResponse.Bindings = bindings;

            // Data.Policy response = await request.ExecuteAsync();

            // Set New Policy with New Owner
            Data2.SetIamPolicyRequest requestBodyIamSet = new Data2.SetIamPolicyRequest();
            requestBodyIamSet.Policy = policyResponse;

            Google.Apis.CloudResourceManager.v1.ProjectsResource.SetIamPolicyRequest iamRequestSet = managerService.Projects.SetIamPolicy(requestBodyIamSet, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data2.Policy iamResponseSet = iamRequestSet.Execute();
        }

        public void CreateNewBillingIam(ConnectedServices _googleConnect, string resource, string role, string member)
        {

            var _cloudbillingService = _googleConnect.GetCloudbillingService();

            BillingAccountsResource.GetIamPolicyRequest getIamRequest = _cloudbillingService.BillingAccounts.GetIamPolicy(resource);

            Data.Policy policyResponse = getIamRequest.Execute();

            IList<Data.Binding> bindings = policyResponse.Bindings;

            // Check if the bindings has the role specified already
            bool has = bindings.Any(binding => binding.Role == role);

            if (has)
            {
                // Get the first binding that has the specified role and add the member to it
                var binding = bindings.First(tempBinding => tempBinding.Role == role);

                if (!binding.Members.Contains(member))
                    bindings.First(tempBinding => tempBinding.Role == role).Members.Add(member);
                else  // The user specified is already added, exit function
                    return;

            }
            else // no binding exists for this role type
            {
                Data.Binding binding = new Data.Binding();
                binding.Role = role;
                binding.Members = new List<string>() { member };

                bindings.Add(binding);
            }

            policyResponse.Bindings = bindings;

            // Set New Policy with New Owner
            Data.SetIamPolicyRequest requestBodyIamSet = new Data.SetIamPolicyRequest();
            requestBodyIamSet.Policy = policyResponse;

            BillingAccountsResource.SetIamPolicyRequest iamRequestSet = _cloudbillingService.BillingAccounts.SetIamPolicy(requestBodyIamSet, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.Policy iamResponseSet = iamRequestSet.Execute();

        }

    }
}