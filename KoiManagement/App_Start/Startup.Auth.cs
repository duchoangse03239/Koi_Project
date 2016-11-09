using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;

namespace KoiManagement
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            //app.UseFacebookAuthentication(
            //   appId: "216790255407823",
            //   appSecret: "fd230a5737cc01dae0c6587062abbc11");


            //app.UseGoogleAuthentication();

            var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions
            {
                AppId = "216790255407823",
                AppSecret = "fd230a5737cc01dae0c6587062abbc11",
                UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name",
                BackchannelHttpHandler = new FacebookBackChannelHandler(),
                Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                        return Task.FromResult(0);
                    }
                }
            };
            facebookOptions.Scope.Add("email");
            //facebookOptions.Scope.Add("phone");
            //facebookOptions.Scope.Add("public_profile");
            app.UseFacebookAuthentication(facebookOptions);

            var googleOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "341271641376-5en326m5vjr6q1k2sfl0dl1o8o99pd9k.apps.googleusercontent.com",
                ClientSecret = "sKoiSSmt7jORFVwvFN5JBDhK",
                CallbackPath = new PathString("/oauth-redirect/google"),
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("GoogleAccessToken", context.AccessToken));
                        return Task.FromResult(0);
                    }
                }
            };

            app.UseGoogleAuthentication(googleOptions);
        }
        public class FacebookBackChannelHandler : HttpClientHandler
        {
            protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                if (!request.RequestUri.AbsolutePath.Contains("/oauth"))
                {
                    request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("?access_token", "&access_token"));
                }

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}