using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Account
{
    public partial class AccountController
    {
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);

            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = await UserProvisioner.ProvisionUserAsync(provider, providerUserId, claims.ToList(), CancellationToken.None);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie
            // this is typically used to store data needed for sign-out from those protocols
            var additionalLocalClaims = new List<Claim>();

            var localSignInProps = new AuthenticationProperties();

            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var identityServerUser = new IdentityServerUser(user.Id)
            {
                DisplayName = user.UserName,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await SignInManager.SignInAsync(user, localSignInProps.IsPersistent);

            // delete temporary cookie used during external authentication
            // await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await Interaction.GetAuthorizationContextAsync(returnUrl);

            await Events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(User user, string provider, string providerUserId, IEnumerable<Claim> claims)> FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();

            claims.Remove(userIdClaim);

            var loginProvider = result.Properties.Items["scheme"];
            var providerKey = userIdClaim.Value;

            // find external user
            var user = await UserLoginStore.FindByLoginAsync(loginProvider, providerKey, CancellationToken.None);

            return (user, loginProvider, providerKey, claims);
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault((claim) => claim.Type == JwtClaimTypes.SessionId);

            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for sign-out
            var idToken = externalResult.Properties.GetTokenValue("id_token");

            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}
