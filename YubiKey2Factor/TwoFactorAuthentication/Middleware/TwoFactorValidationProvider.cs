using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Google.Authenticator;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using TwoFactorAuthentication.Models;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.Identity;


namespace TwoFactorAuthentication.Middleware
{
    public class TwoFactorValidationProvider : DataProtectorTokenProvider<BackOfficeIdentityUser, int>, IUserTokenProvider<BackOfficeIdentityUser, int>
    {
        public TwoFactorValidationProvider(IDataProtector protector) : base(protector)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Explicitly implement this interface method - which overrides the base class's implementation
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<bool> IUserTokenProvider<BackOfficeIdentityUser, int>.ValidateAsync(string purpose, string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            if (purpose == Constants.GoogleAuthenticatorProviderName)
            {
                var twoFactorAuthenticator = new TwoFactorAuthenticator();

                using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
                {
                    var result = await scope.Database.Query<TwoFactor>()
                        .Where(x => x.UserId == user.Id && x.Key == Constants.GoogleAuthenticatorProviderName && x.Confirmed)
                        .ToListAsync();

                    if (result.Any() == false)
                        return false;

                    var key = result.First().Value;
                    var validToken = twoFactorAuthenticator.ValidateTwoFactorPIN(key, token);
                    return validToken;
                }
            }

            /* if (purpose == Constants.YubiKeyProviderName)
             {
                 var yubiKeyService = new YubiKeyService();
                 var response = yubiKeyService.Validate(token, user.Id);
                 return Task.FromResult(response != null && response.Status == YubicoResponseStatus.Ok);
             }*/

            return false;
        }
    }
}