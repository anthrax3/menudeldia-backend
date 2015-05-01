using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using MenuDelDia.Entities;
using MenuDelDia.Presentacion;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace MenuDelDia.API.OAuthProvider
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {


        public SimpleAuthorizationServerProvider()
        {

        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var applicationSignInManager = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();

            var user = await applicationSignInManager.UserManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "Usuario o contraseña no válidos.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", user.Email));
            identity.AddClaim(new Claim("id", user.Id.ToString()));
            identity.AddClaim(new Claim("restaurantId", user.RestaurantId.ToString()));
            identity.AddClaim(new Claim("role", "user"));

            AuthenticationProperties properties = CreateProperties(user);
            var ticket = new AuthenticationTicket(identity, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var prop in context.Properties.Dictionary)
            {
                if (context.AdditionalResponseParameters.ContainsKey(prop.Key) == false)
                    context.AdditionalResponseParameters.Add(prop.Key, prop.Value);
            }

            return base.TokenEndpoint(context);
        }

        public static AuthenticationProperties CreateProperties(ApplicationUser user)
        {
            //var firstName = user.FirstName;
            //var lastName = user.LastName;

            //if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            //{
            //    firstName = user.Email;
            //    lastName = string.Empty;
            //}
            //else if (string.IsNullOrEmpty(firstName) == false && string.IsNullOrEmpty(lastName))
            //{
            //    lastName = string.Empty;
            //}

            //IDictionary<string, string> data = new Dictionary<string, string>
            //{
            //    {"firstName", firstName},
            //    {"lastName", lastName},
            //    {"email", user.Email},
            //};

            IDictionary<string, string> data = new Dictionary<string, string>();
            return new AuthenticationProperties(data);
        }
    }
}