Note:
- Currently deployed in App Engine version: 20201005t103241, of a single instance.

- In case of republishing this app (other version or same version) please consider the following:
	- please use the current app.yaml. 
	- If variable "instances" edited to more than one (1), please consider the effect of app engine auto scaling:
		- you need a separated db. Edit the correct connection string from appsettings.json file. Also you need to download the correct library to support that db.
		- you need to configure the app.yaml in such away it will support consistent session for current login user (for admin module).
	- If republish (even if in the same version), the DB will be deleted (currently using sqlite).
	- Edit the app.yaml environment variables. 
	- If publish to other cloud other than google: 
		- you need to setup the environment variable GOOGLE_APPLICATION_CREDENTIALS with the path where service account is physically located.
		- you need to manually upload the service account credential in a safe localtion. Make user the GOOGLE_APPLICATION_CREDENTIALS value match the correct path of the service account.

Sample app.yaml
====================================================
runtime: aspnetcore
env: flex

env_variables:
  AuthenticationGoogleClientEmail: [please replace this with service account client Email]
  AuthenticationGoogleClientId: [Please replace this with (OAuth2) Client Id]
  AuthenticationGoogleClientSecret: [Please replace this with (OAuth2) Client Secret]
  AppRedirectURL: [Please replace this with valid redirect URL set from OAuth2 configuration]
 
manual_scaling:
  instances: 1

resources:
  cpu: 1
  memory_gb: 2
====================================================
