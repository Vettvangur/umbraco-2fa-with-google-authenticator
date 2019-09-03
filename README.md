# Umbraco 2fa plugin with Google authenticator for v8
2FA with Google Authenticator. Forked from ng-soft's v7 implementation and modified to work with v8. Sebastian's original version supported YubiKey and Google Authenticator. This version contains Google Authenticator only.

## Notes

* This version has been built against v8.1.1.
* There is a migration to add a new custom table to the database. The table is named TwoFactor.
* Installation will add a new dashboard for enabling 2FA for the current user.

## Usage

To test the 2FA you can run the project. This will run the install process and setup a new site. 

### Existing Project

To add 2FA to an existing project
1. Change the ApplicationName constant in TwoFactorAuthentication\constants.cs (this is the application name that appears in Google Authentictor) 
2. Build the project
3. Copy YubiKey2Factor.dll, YubicoDotNetClient.dll, Google.Authenticator.dll to existing site's bin directory
4. Copy the App_Plugin\2FactorAuthentication directory to the existing site's App_Plugin directory

## Upgrade

The upgrade to v8 required 
1. Modifying the migration to use the v8 method
2. Moving the startup events to a component (and composer)
3. Moving all of the database calls to a service so the database scope can be used.
4. Adding configuration to the package.manifest for loading the dashboard.
5. Updating method parameters as many of the Umbraco authentication methods have changed their signature.
6. Changing dialogService to editorServce in the Angular login controller.

## Issues

The two factor authentication works but there is an issue as the user is not redirected after validating during initial setup or after entering the 2FA code during the login. The user is authenticated but needs to manually refresh the page. The issue is with the Angular view.

More details are in issue #1.
