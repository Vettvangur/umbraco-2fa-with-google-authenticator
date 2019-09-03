using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Google.Authenticator;
using TwoFactorAuthentication.Models;

using Umbraco.Web.WebApi;

using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Umbraco.Web.Editors;
using System.Net;
using Umbraco.Core;
using Umbraco.Core.Security;

public class TwoFactorAuthController : UmbracoAuthorizedApiController
{
    private readonly TwoFactorService _twoFactorService;

    public TwoFactorAuthController(TwoFactorService twoFactorService)
    {
        _twoFactorService = twoFactorService;
    }

    [HttpGet]
    public List<TwoFactorAuthInfo> TwoFactorEnabled()
    {
      
        var user = Security.CurrentUser;

        var result =_twoFactorService.GetTwoFactorEnabled(user.Id);
       
        return result;
    }

    [HttpGet]
    public TwoFactorAuthInfo GoogleAuthenticatorSetupCode()
    {
        var tfa = new TwoFactorAuthenticator();
        var user = Security.CurrentUser;
        var accountSecretKey = Guid.NewGuid().ToString();

        var setupInfo = tfa.GenerateSetupCode(Constants.ApplicationName, user.Email, accountSecretKey, 300, 300);

        var twoFactorAuthInfo = _twoFactorService.GetExistingAccount(user.Id, Constants.GoogleAuthenticatorProviderName, accountSecretKey);

        twoFactorAuthInfo.Secret = setupInfo.ManualEntryKey;
        twoFactorAuthInfo.Email = user.Email;
        twoFactorAuthInfo.ApplicationName = Constants.ApplicationName;
        twoFactorAuthInfo.QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl;

        return twoFactorAuthInfo;
    }

    [HttpPost]
    public bool ValidateAndSaveGoogleAuth(string code)
    {
        var user = Security.CurrentUser;
        return  _twoFactorService.ValidateAndSaveGoogleAuth(code, user.Id);
    }

    [HttpPost]
    public bool Disable()
    {
        var result = 0;
        var isAdmin =  Security.CurrentUser.Groups.Select(x => x.Name == "Administrators").FirstOrDefault();
        if (isAdmin)
        {
            var user = Security.CurrentUser;
             result = _twoFactorService.Disable(user.Id);
            //if more than 0 rows have been deleted, the query ran successfully
        }
        return result != 0;
    }
}

