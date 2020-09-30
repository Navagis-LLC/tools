using System;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util;
using Google.Apis.Requests.Parameters;
using Google.Apis.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.Services;
using Google.Apis.Cloudbilling.v1;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

using Data = Google.Apis.CloudResourceManager.v1.Data;
using BillingData = Google.Apis.Cloudbilling.v1.Data;
using Newtonsoft.Json;

using RegisterProject_Spice.Pages.Models;
using System.Text.Json.Serialization;
using Google.Apis.Oauth2.v2;

namespace RegisterProject_Spice.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public List<Data.Project> Projects { get; set; }

        // This is the Billing Account ID to change the Project to.  
        // This should be a subaccount under Navagis created specifically for this customer
        [BindProperty(SupportsGet = true)]
        public string BAI { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        [BindProperty(SupportsGet = true)]
        public string variables { get; set; }

        [BindProperty(SupportsGet = true)]
        public string isDisabled { get; set; }

        [BindProperty(SupportsGet = true)]
        public string projectDisplayVisibility { get; set; }

        private IConfiguration Configuration { get; }

        // We assume this is the same service account email that the google credentials are coming from
        protected static string serviceAccount { get; set; }
        protected static string clientID { get; set; }
        protected static string clientSecret { get; set; }

        // localhost
        // protected static string redirect_uri = "https://localhost:44362/";

        // App Engine
        protected const string redirect_uri = "https://project-auth-211619.appspot.com";

        // Azure
        // protected const string redirect_uri = "http://registerproject-spice20180729043940.azurewebsites.net";


        private ApplicationDbContext db;

        public IndexModel(IConfiguration configuration, ApplicationDbContext context)
        {
            Configuration = configuration;
            serviceAccount = Configuration["Authentication:Google:ClientEmail"];
            clientID = Configuration["Authentication:Google:ClientId"];
            clientSecret = Configuration["Authentication:Google:ClientSecret"];
            db = context;
        }


        public async void OnGet()
        {

            Projects = new List<Data.Project>();
            isDisabled = "";
            projectDisplayVisibility = "hidden";

            /*if (BAI != null)
                HttpContext.Session.SetString("BAI", BAI);

            if (HttpContext.Session.GetString("BAI") != null)
                variables = HttpContext.Session.GetString("BAI");*/

            if (Code != null)
            {
                // Retrieve Token and set to Session
                this.HttpContext.Session = await setGoogleToken(Code, this.HttpContext.Session);
                
                //  var boolvalue = (bool)HttpContext.Items["isVerified"];
                var code2 =   this.HttpContext.Session.GetString("Token");

                // Get Email Address
                var Email = GetEmail(code2);
                Client client = db.Clients.SingleOrDefault(c => c.Email == Email);
                if (client != null)
                {
                    BillingAccount billingAccount = db.BillingAccounts.SingleOrDefault(b => b.Id == client.BillingAccountId);
                    if (billingAccount != null)
                    {
                        BAI = billingAccount.BillingAccountName;
                        HttpContext.Session.SetString("BAI", BAI);
                        variables = HttpContext.Session.GetString("BAI");
                    }
                }
                
                CloudResourceManagerService cloudResourceManagerService = new CloudResourceManagerService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = await GetCredential(this.HttpContext.Session),
                    ApplicationName = "Google-CloudResourceManagerSample/0.1",
                });

                Google.Apis.CloudResourceManager.v1.ProjectsResource.ListRequest request = cloudResourceManagerService.Projects.List();
                Data.ListProjectsResponse response;

                // To execute asynchronously in an async method, replace `request.Execute()` as shown:
                response = request.Execute();
                // response = await request.ExecuteAsync();

                isDisabled = "disabled";
                projectDisplayVisibility = "visible";
                Projects = response.Projects.ToList<Data.Project>();
            }

            // TODO: Specify Error if no BAI given
            // TODO: Check to see if BAI given is under the navagis master account, if it's not, give an error
            // TODO: Check to see if BAI given has the serviceAccount as a billing.admin - if not, give an error
        }

        private string GetEmail(string token)
        {
            var service = new Oauth2Service(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GoogleCredential.FromAccessToken(token),
                    ApplicationName = "Google-CloudResourceManagerSample/0.1"
                }
            );
            var userInfo = service.Userinfo.Get().Execute();
            HttpContext.Session.SetString("Username_email", userInfo.Email);
            return userInfo.Email;   
        }
        
        public async Task<IActionResult> CreateNewBillingAcct(string resource, ISession session)
        {
            CloudbillingService cloudbillingService = new CloudbillingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredential(session),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });
            
            // List Projects
            BillingAccountsResource.ListRequest billingAccountListRequest = cloudbillingService.BillingAccounts.List();

            // ProjectsResource.ListRequest request = cloudResourceManagerService.Projects.List();
            BillingData.ListBillingAccountsResponse listAccountsResponse;
            listAccountsResponse = billingAccountListRequest.Execute();
            BillingData.ProjectBillingInfo billingInfo = new BillingData.ProjectBillingInfo();
            billingInfo.BillingAccountName = listAccountsResponse.BillingAccounts[0].Name;
            billingInfo.Name = "projects/" + resource + "/billingInfo";

            // IClientService
            Google.Apis.Cloudbilling.v1.ProjectsResource.UpdateBillingInfoRequest updateBilling = new Google.Apis.Cloudbilling.v1.ProjectsResource.UpdateBillingInfoRequest(cloudbillingService, billingInfo, "projects/" + resource);
            updateBilling.Execute();
            BillingAccountsResource.GetIamPolicyRequest iamPolicyRequest = cloudbillingService.BillingAccounts.GetIamPolicy(listAccountsResponse.BillingAccounts[0].Name);
            BillingData.Policy iamPolicy = iamPolicyRequest.Execute();

            return new JsonResult("hi");
        }

        public void ChangeBillingAccount(string billingAccountName, string resource)
        {
            CloudbillingService cloudbillingService = new CloudbillingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetGoogleCredential(),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });

            BillingData.ProjectBillingInfo billingInfo = new BillingData.ProjectBillingInfo();
            billingInfo.BillingAccountName = billingAccountName;
            billingInfo.Name = "projects/" + resource + "/billingInfo";
            Google.Apis.Cloudbilling.v1.ProjectsResource.UpdateBillingInfoRequest updateBilling = new Google.Apis.Cloudbilling.v1.ProjectsResource.UpdateBillingInfoRequest(cloudbillingService, billingInfo, "projects/" + resource);
            updateBilling.Execute();
            
        }

        public List<BillingData.BillingAccount> GetBillingAccounts()
        {
            CloudbillingService cloudbillingService = new CloudbillingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetGoogleCredential(),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });
            BillingAccountsResource.ListRequest billingAccountListRequest = cloudbillingService.BillingAccounts.List();
            BillingData.ListBillingAccountsResponse listAccountsResponse;
            listAccountsResponse = billingAccountListRequest.Execute();
            return listAccountsResponse.BillingAccounts.ToList<BillingData.BillingAccount>();

        }

        public void CreateNewBillingIam(string resource, string role, string member)
        {
            CloudbillingService cloudbillingService = new CloudbillingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetGoogleCredential(),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });

            BillingAccountsResource.GetIamPolicyRequest getIamRequest = cloudbillingService.BillingAccounts.GetIamPolicy(resource);
            BillingData.Policy policyResponse = getIamRequest.Execute();
            IList<BillingData.Binding> bindings = policyResponse.Bindings;

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
                BillingData.Binding binding = new BillingData.Binding();
                binding.Role = role;
                binding.Members = new List<string>() { member };
                bindings.Add(binding);
            }

            policyResponse.Bindings = bindings;

            // Set New Policy with New Owner
            BillingData.SetIamPolicyRequest requestBodyIamSet = new BillingData.SetIamPolicyRequest();
            requestBodyIamSet.Policy = policyResponse;
            BillingAccountsResource.SetIamPolicyRequest iamRequestSet = cloudbillingService.BillingAccounts.SetIamPolicy(requestBodyIamSet, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            BillingData.Policy iamResponseSet = iamRequestSet.Execute();
        }

        public void removeNewBillingIam(string resource, string role, string member)
        {

            CloudbillingService cloudbillingService = new CloudbillingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetGoogleCredential(),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });

            BillingAccountsResource.GetIamPolicyRequest getIamRequest = cloudbillingService.BillingAccounts.GetIamPolicy(resource);
            BillingData.Policy policyResponse = getIamRequest.Execute();
            IList<BillingData.Binding> bindings = policyResponse.Bindings;

            // Check if the bindings has the role specified already
            bool has = bindings.Any(binding => binding.Role == role);
            if (has)
            {
                // Get the first binding that has the specified role and add the member to it
                var binding = bindings.First(tempBinding => tempBinding.Role == role);

                if (!binding.Members.Contains(member))
                    return; // EXIT - can't find the member
                else  // The user specified exists, remove him
                    binding.Members.Remove(member);

            }
            else // no binding exists for this role type
            {
                return; // EXIT no binding for this member exists
            }
            policyResponse.Bindings = bindings;
            // Set New Policy with New Owner
            BillingData.SetIamPolicyRequest requestBodyIamSet = new BillingData.SetIamPolicyRequest();
            requestBodyIamSet.Policy = policyResponse;
            BillingAccountsResource.SetIamPolicyRequest iamRequestSet = cloudbillingService.BillingAccounts.SetIamPolicy(requestBodyIamSet, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            BillingData.Policy iamResponseSet = iamRequestSet.Execute();
        }

        public void CreateNewProjectIam(string resource, CloudResourceManagerService cloudResourceManagerService, string role, string member)
        {
            // TODO: Assign values to desired properties of `requestBody`:
            Data.GetIamPolicyRequest requestBody = new Data.GetIamPolicyRequest();
            Google.Apis.CloudResourceManager.v1.ProjectsResource.GetIamPolicyRequest iamRequest = cloudResourceManagerService.Projects.GetIamPolicy(requestBody, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.Policy policyResponse = iamRequest.Execute();
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

            // It seems we don't need to do this since bindings is already == policyResponse.Bindings
            policyResponse.Bindings = bindings;

            // Data.Policy response = await request.ExecuteAsync();

            // Set New Policy with New Owner
            Data.SetIamPolicyRequest requestBodyIamSet = new Data.SetIamPolicyRequest();
            requestBodyIamSet.Policy = policyResponse;
            Google.Apis.CloudResourceManager.v1.ProjectsResource.SetIamPolicyRequest iamRequestSet = cloudResourceManagerService.Projects.SetIamPolicy(requestBodyIamSet, resource);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.Policy iamResponseSet = iamRequestSet.Execute();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostChangeBillingAsync(string Resource)
        {

            if (HttpContext.Session.GetString("BAI") == null)
                return new JsonResult("BAI_ERROR");
  
                string Bai = HttpContext.Session.GetString("BAI");

            // Get the credentials from the current logged in user, i assume they are already logged in, but get again anyway
            CloudResourceManagerService cloudResourceManagerService = new CloudResourceManagerService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredential(this.HttpContext.Session),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });

            string billingAccountName = "billingAccounts/" + Bai;

            try
            {
                // Add the serviceAccount as Owner to the Chosen Billing Account
                CreateNewProjectIam(Resource, cloudResourceManagerService, "roles/owner", "serviceAccount:" + serviceAccount);

                // Add nav-cloud-support group as Editor
                CreateNewProjectIam(Resource, cloudResourceManagerService, "roles/editor", "group:" + "nav-cloud-support@navagis.com");

                // Add nav-cloud-support as Iam Admin
                CreateNewProjectIam(Resource, cloudResourceManagerService, "roles/resourcemanager.projectIamAdmin", "group:" + "nav-cloud-support@navagis.com");

                // Add nav-cloud-viewer as viewers
                CreateNewProjectIam(Resource, cloudResourceManagerService, "roles/viewer", "group:" + "nav-cloud-viewer@navagis.com");

                // Move the Billing Account to the Given Navagis SubAccount in the BAI URL
                ChangeBillingAccount(billingAccountName, Resource);

                //TODO: Before we clean up, make sure the Project was correctly moved to the right billingAccount
                //Cleanup the Billing User Accounts
                CreateNewBillingIam(billingAccountName, "roles/billing.admin", "user:david@navagis.com");
                CreateNewBillingIam(billingAccountName, "roles/billing.admin", "group:billing@navagis.com");
                removeNewBillingIam(billingAccountName, "roles/billing.admin", "serviceAccount:" + serviceAccount);

                Projects = new List<Data.Project>();

                return new JsonResult(JsonConvert.SerializeObject(Projects));
            }
            catch (Exception)
            {
                return new JsonResult("BAI_PERMISSION_ERROR");
            }
     
        }

        public async Task<IActionResult>  getGoogleRedirectUri()
        {
   
            var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientID,
                    ClientSecret = clientSecret
                },
                Scopes = new[] {
                    CloudResourceManagerService.Scope.CloudPlatform,
                    "https://www.googleapis.com/auth/userinfo.email"
                }
            };
            IAuthorizationCodeFlow flow2 = new GoogleAuthorizationCodeFlow(flowInitializer);
            GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(flowInitializer);
            AuthorizationCodeRequestUrl url = flow.CreateAuthorizationCodeRequest("test");
            var blah = new AuthorizationCodeWebApp(flow2, redirect_uri, "state1");
            AuthorizationCodeWebApp.AuthResult result = await blah.AuthorizeAsync("user", new System.Threading.CancellationToken());
            
            return new JsonResult(result.RedirectUri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAuthGoogleAsync(string Name)
        {
            return await getGoogleRedirectUri();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPopulateProjectsAsync(string Name)
        {
            CloudResourceManagerService cloudResourceManagerService = new CloudResourceManagerService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredential(this.HttpContext.Session),
                ApplicationName = "Google-CloudResourceManagerSample/0.1",
            });

            Google.Apis.CloudResourceManager.v1.ProjectsResource.ListRequest request = cloudResourceManagerService.Projects.List();
            Data.ListProjectsResponse response;

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            response = request.Execute();

            if (response.Projects == null)
            {
                return null;
            }
            var x = from project in response.Projects select new { ProjectId = project.ProjectId, ProjectName = project.Name, CreatedDate = project.CreateTime, State = project.LifecycleState };
            return new JsonResult(JsonConvert.SerializeObject(x));
        }

        // Set the Google Token from Code
        // Return token if succeeded
        public async Task<ISession> setGoogleToken(string Code, ISession session)
        {
            var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientID,
                    ClientSecret = clientSecret
                },
                Scopes = new[] { CloudResourceManagerService.Scope.CloudPlatform, "https://www.googleapis.com/auth/userinfo.email" }
            };
            IAuthorizationCodeFlow flow = new AuthorizationCodeFlow(flowInitializer);
            var token = await exchangeCodeForTokenAsync(Code, redirect_uri, new System.Threading.CancellationToken());
            session.SetString("Token", token.AccessToken);
            return session;
        }

        public static async Task<TokenResponse> exchangeCodeForTokenAsync(string Code, string redirectUri, System.Threading.CancellationToken cancellationToken)
        {
            var authorizationCodeTokenReq = new AuthorizationCodeTokenRequest
            {
                Scope = string.Join(" ", new[] { CloudResourceManagerService.Scope.CloudPlatform, "https://www.googleapis.com/auth/userinfo.email" }),
                RedirectUri = redirect_uri,
                Code = Code,
            };
            var token = await FetchTokenAsync("user", authorizationCodeTokenReq, new System.Threading.CancellationToken()).ConfigureAwait(false);
            return token;
        }

        public static async Task<TokenResponse> FetchTokenAsync(string userId, TokenRequest request,
            System.Threading.CancellationToken taskCancellationToken)
        {
            // Add client id and client secret to requests.
            request.ClientId = clientID;
            request.ClientSecret = clientSecret;
            IClock Clock = SystemClock.Default;
            WebClient client = new WebClient();
            
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] responseArray = client.UploadData("https://www.googleapis.com/oauth2/v4/token", await ParameterUtils.CreateFormUrlEncodedContent(request).ReadAsByteArrayAsync());
            string response2 = Encoding.ASCII.GetString(responseArray);
        
            // Gets the token and sets its issued time.
            var newToken = NewtonsoftJsonSerializer.Instance.Deserialize<TokenResponse>(response2); // changed to response2
            newToken.IssuedUtc = Clock.UtcNow;
            return newToken;
        }

        // This is a pretty terrible function, we must have a session variable set for code or token for this to work
        public async static Task<GoogleCredential> GetCredential(ISession session)
        {
            // if we have a token, return a credential
            if (session.GetString("Token") != null)
            {
                return GoogleCredential.FromAccessToken(session.GetString("Token"));
            }

            // if we have a Code, get a token, then return a credential
            if (session.GetString("Code") != null)
            {
                var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientID,
                        ClientSecret = clientSecret
                    },
                    Scopes = new[] { CloudResourceManagerService.Scope.CloudPlatform }
                };
                IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(flowInitializer);
                var token = await exchangeCodeForTokenAsync(session.GetString("Code"), redirect_uri, new System.Threading.CancellationToken());
                return GoogleCredential.FromAccessToken(token.AccessToken);
            }
            return null;
        }

        public static GoogleCredential GetGoogleCredential()
        {
            var credential = GoogleCredential.GetApplicationDefault();
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }
            return credential;
        }
    }
}


