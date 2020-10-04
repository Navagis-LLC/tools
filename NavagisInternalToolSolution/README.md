Note:
- Currently deployed in App Engine version: 20201003t100456, of a single instance.

- In case of republishing this app (other version or same version) please consider the following:
	- please use to the current app.yaml. 
	- If variable "instances" edited to more than 1, please consider effect of app engine auto scaling:
		- you need to configure a separated db. Edit the correct connection string from appsettings.json. Also you need to download the correct library to support that db.
		- you need to confiture the app.yml in such away it will support consistent session for login user (for admin module).
	- If republish (even if in the same version), the DB will be deleted.
	- Edit the app.yaml environment variables. 

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
