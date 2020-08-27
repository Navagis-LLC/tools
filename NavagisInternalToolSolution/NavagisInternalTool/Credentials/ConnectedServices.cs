using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.Cloudbilling.v1;
using Google.Apis.Services;

namespace NavagisInternalTool.Credentials
{
    public class ConnectedServices
    {
        private AuthorizationCodeMvcApp.AuthResult _authorizationCodeApp;
        private readonly string _applicationName = "Myprojects";
        private Controller _controller;
        private CancellationToken _cancellationToken;

        public ConnectedServices(Controller controller, CancellationToken cancellationToken)
        {
            _controller = controller;
            _cancellationToken = cancellationToken;
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
                    HttpClientInitializer = _authorizationCodeApp.Credential,
                    ApplicationName = _applicationName
                }
            );
            return service;
        }
    }
}