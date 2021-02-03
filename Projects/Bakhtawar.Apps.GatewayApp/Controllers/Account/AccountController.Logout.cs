using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Models.Account;
using Bakhtawar.Apps.GatewayApp.ViewModels.Account;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Account
{
    public partial class AccountController
    {
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var model = await BuildLogoutViewModelAsync(logoutId);

            if (model.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly
                return await Logout(model);
            }

            return View("LoggingOut", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var viewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity?.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await SignInManager.SignOutAsync();

                // raise the logout event
                await Events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (viewModel.TriggerExternalSignOut)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing
                var url = Url.Action("Logout", new { logoutId = viewModel.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, viewModel.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", viewModel);
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var viewModel = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity?.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                viewModel.ShowLogoutPrompt = false;

                return viewModel;
            }

            var context = await Interaction.GetLogoutContextAsync(logoutId);

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                viewModel.ShowLogoutPrompt = false;

                return viewModel;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return viewModel;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated sign-out)
            var logout = await Interaction.GetLogoutContextAsync(logoutId);

            var viewModel = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignOut = await HttpContext.GetSchemeSupportsSignOutAsync(idp);

                    if (providerSupportsSignOut)
                    {
                        if (viewModel.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we sign-out and redirect away to the external IdP for sign-out
                            viewModel.LogoutId = await Interaction.CreateLogoutContextAsync();
                        }

                        viewModel.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return viewModel;
        }
    }
}
