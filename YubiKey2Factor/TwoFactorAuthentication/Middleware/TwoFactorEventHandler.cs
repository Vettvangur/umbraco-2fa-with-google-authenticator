﻿using System;
using Owin;
using Umbraco.Core;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Web;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using NPoco;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Security;
using Umbraco.Web.WebApi;

namespace TwoFactorAuthentication.Middleware
{
    public sealed class TwoFactorEventHandler : IComponent
    {
        private readonly IUmbracoContextAccessor umbracoContextAccessor; private readonly IRuntimeState runtimeState; private readonly IUserService userService; private readonly IGlobalSettings globalSettings; private readonly ISecuritySection securitySection; private readonly IEntityService entityService; private readonly IExternalLoginService externalLoginService; private readonly IMemberTypeService memberTypeService; private readonly UmbracoMapper umbracoMapper;

        public TwoFactorEventHandler(
            IUmbracoContextAccessor umbracoContextAccessor,
            IRuntimeState runtimeState,
            IUserService userService,
            IGlobalSettings globalSettings,
            ISecuritySection securitySection,
            IEntityService entityService,
            IExternalLoginService externalLoginService,
            IMemberTypeService memberTypeService,
            UmbracoMapper umbracoMapper)
        {
            this.umbracoContextAccessor = umbracoContextAccessor;
            this.runtimeState = runtimeState;
            this.userService = userService;
            this.globalSettings = globalSettings;
            this.securitySection = securitySection;
            this.entityService = entityService;
            this.externalLoginService = externalLoginService;
            this.memberTypeService = memberTypeService;
            this.umbracoMapper = umbracoMapper;
        }

        private void ConfigureTwoFactorAuthentication(object sender, OwinMiddlewareConfiguredEventArgs args)
        {
            var app = args.AppBuilder;
            var applicationContext = Umbraco.Core.Composing.Current.Services;

            IGlobalSettings GlobalSettings = Umbraco.Core.Composing.Current.Configs.Global();
            IUmbracoSettingsSection UmbracoSettings = Umbraco.Core.Composing.Current.Configs.Settings();
            UmbracoMapper umbracoMapper = Umbraco.Core.Composing.Current.Mapper;
            //netser/////////////////////////
            /* var oAuthServerOptions = new OAuthAuthorizationServerOptions

             {
                 AllowInsecureHttp = true,
                 TokenEndpointPath = new PathString("/token"),
                 AccessTokenExpireTimeSpan = TimeSpan.FromDays(1)
             };
             // Token Generation
             app.UseOAuthAuthorizationServer(oAuthServerOptions);
             app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());*/
            ////////////////////end netser

            app.SetUmbracoLoggerFactory();
            app.UseTwoFactorSignInCookie(Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType, TimeSpan.FromMinutes(5));

            // app.UseOAuthAuthorizationServer(options);
            // app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            // We need to set these values again after our custom changes. Otherwise preview doesn't work.
            // Gunni: Apparently we don't need this for preview to work and the following code breaks other Identity providers
            //app.UseUmbracoBackOfficeCookieAuthentication(umbracoContextAccessor, Umbraco.Web.Composing.Current.RuntimeState, applicationContext.UserService, GlobalSettings, securitySection)
            //    .UseUmbracoBackOfficeExternalCookieAuthentication(umbracoContextAccessor, runtimeState, GlobalSettings)
            //    .UseUmbracoPreviewAuthentication(umbracoContextAccessor, runtimeState, globalSettings, securitySection);

            app.ConfigureUserManagerForUmbracoBackOffice<TwoFactorBackOfficeUserManager, BackOfficeIdentityUser>(
                Umbraco.Web.Composing.Current.RuntimeState,
                GlobalSettings,
                (options, context) =>
                {
                    var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                    var userManager = TwoFactorBackOfficeUserManager.Create(options,
                        applicationContext.UserService,
                        applicationContext.MemberTypeService,
                        applicationContext.EntityService,
                        applicationContext.ExternalLoginService,
                        membershipProvider, GlobalSettings, umbracoMapper );
                    return userManager;
                });
        }

        public void Initialize()
        {
        UmbracoDefaultOwinStartup.MiddlewareConfigured += ConfigureTwoFactorAuthentication;
    }

        public void Terminate()
        {
           
        }
    }
}