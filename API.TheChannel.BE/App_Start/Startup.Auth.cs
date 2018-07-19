using System;
using System.Configuration;
using System.Threading.Tasks;
using API.TheChannel.BE.Infrastructure;
using API.TheChannel.BE.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Microsoft.Owin.Security.Jwt;

namespace API.TheChannel.BE
{
    public partial class Startup
    {

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user/Role manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            //PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth2/Token"),

                //Provider = new ApplicationOAuthProvider(PublicClientId),
                Provider = new CustomOAuthProvider(),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),

                AccessTokenFormat = new CustomJwtFormat(System.Configuration.ConfigurationManager.AppSettings["BaseUrlAddress"]),

                //############################ IMPORTANT PRODUCTION ###########################
                //JB In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
                //############################ IMPORTANT PRODUCTION ###########################
            };

            // Enable the application to use bearer tokens to authenticate users



            var issuer = ConfigurationManager.AppSettings["BaseUrlAddress"];
            var audience = ConfigurationManager.AppSettings["Client_Id"];
            var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["Secret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    },
                    Provider = new OAuthBearerAuthenticationProvider
                    {
                        OnValidateIdentity = context =>
                        {
                            context.Ticket.Identity.AddClaim(new System.Security.Claims.Claim("newCustomClaim", "newValue"));
                            return Task.FromResult<object>(null);
                        }
                    }
                });

            app.UseOAuthAuthorizationServer(OAuthOptions);
        }

    }
}