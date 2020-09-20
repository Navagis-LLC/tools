using System;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.Cloudbilling.v1;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;

using NavagisInternalTool.Models;


namespace NavagisInternalTool.Credentials
{
    public class ConnectedServices
    {
        private AuthorizationCodeMvcApp.AuthResult _authorizationCodeApp;
        private static int _settingRecordID = Convert.ToInt32(WebConfigurationManager.AppSettings["DBDefaultSettingID"]);
        private static Setting _setting = new ApplicationDBContext().Settings.SingleOrDefault(s => s.Id == _settingRecordID);

        private readonly string _applicationName = "Google-CloudResourceManagerSample/0.1";
        private Controller _controller;
        private CancellationToken _cancellationToken;

        public ConnectedServices(Controller controller, CancellationToken cancellationToken)
        {
            _controller = controller;
            _cancellationToken = cancellationToken;
        }

        public AuthorizationCodeMvcApp.AuthResult GetAuthResult()
        {
            return _authorizationCodeApp;
        }

        public async Task<bool> Authorize()
        {
            var _isAuthorize = true;

            _authorizationCodeApp = await new AuthorizationCodeMvcApp(_controller, new AppFlowMetadata())
                .AuthorizeAsync(_cancellationToken);

            if (_authorizationCodeApp.Credential == null)
                _isAuthorize = false;
            
            return _isAuthorize;

        }

        public Oauth2Service GetOauth2Service()
        {
            var service = new Oauth2Service(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _authorizationCodeApp.Credential,
                    ApplicationName = _applicationName
                }
            );
            return service;
        }

        public CloudResourceManagerService GetCloudResourceManagerService()
        {
            CloudResourceManagerService service = new CloudResourceManagerService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _authorizationCodeApp.Credential,
                    ApplicationName = _applicationName
                }
            );
            return service;
        }

        public CloudbillingService GetCloudbillingService()
        {

            CloudbillingService service = new CloudbillingService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetGoogleCredential(),
                    ApplicationName = _applicationName
                }
            );
           
            return service;
        }

        public static GoogleCredential GetGoogleCredential()
        {
            //FileStream fileStream = new FileStream(_setting.ServiceAccountFilePath, FileMode.Open);
            //GoogleCredential credential = GoogleCredential.FromStream(fileStream);

            var credential = GoogleCredential.GetApplicationDefault();

            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }
            return credential;
        }
    }
}