using System;
using System.Web.Mvc;
using System.Linq;
using System.Web.Configuration;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Util.Store;
using Google.Apis.CloudResourceManager.v1;

using NavagisInternalTool.Models;

namespace NavagisInternalTool.Credentials
{
    public class AppFlowMetadata : FlowMetadata
    {
        /*
         * Credential 
         */
        private static string _clientID = new Setting().ClientId;
        private static string _clientSecret = new Setting().ClientSecret;

        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientID,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] 
                {
                    CloudResourceManagerService.Scope.CloudPlatformReadOnly,
                    CloudResourceManagerService.Scope.CloudPlatform
                },
                DataStore = new FileDataStore("NavagisInternalTool")
            });

        public override string GetUserId(Controller controller)
        {
            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();
        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}