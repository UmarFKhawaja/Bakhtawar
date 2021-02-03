using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Models.Account;
using Bakhtawar.Apps.GatewayApp.ViewModels.Account;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Account
{
    public partial class AccountController
    {
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var viewModel = await BuildLoginViewModelAsync(returnUrl);

            if (viewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "Account", new { scheme = viewModel.ExternalLoginScheme, returnUrl });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await Interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await Interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (await PasswordValidator.ValidateCredentialsAsync(model.Email, model.Password))
                {
                    var user = await UserStore.FindByNameAsync(model.Email, CancellationToken.None);

                    await Events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    // only set explicit expiration here if user chooses "remember me"
                    // otherwise we rely upon expiration configured in cookie middleware
                    // var props = (AuthenticationProperties)null;
                    //
                    // if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    // {
                    //     props = new AuthenticationProperties
                    //     {
                    //         IsPersistent = true,
                    //         ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    //     };
                    // };

                    // issue authentication cookie with subject ID and username
                    // var identityServerUser = new IdentityServerUser(user.Id)
                    // {
                    //     DisplayName = user.UserName
                    // };
                    //
                    await SignInManager.SignInAsync(user, AccountOptions.AllowRememberLogin && model.RememberLogin);

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await Events.RaiseAsync(new UserLoginFailureEvent(model.Email, "invalid credentials", clientId:context?.Client.ClientId));

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var viewModel = await BuildLoginViewModelAsync(model);

            return View(viewModel);
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await Interaction.GetAuthorizationContextAsync(returnUrl);

            if (context?.IdP != null && await SchemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var viewModel = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Email = context?.LoginHint,
                };

                if (!local)
                {
                    viewModel.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return viewModel;
            }

            var schemes = await SchemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where((authenticationScheme) => authenticationScheme.DisplayName != null)
                .Select
                (
                    (authenticationScheme) => new ExternalProvider
                    {
                        DisplayName = authenticationScheme.DisplayName ?? authenticationScheme.Name,
                        AuthenticationScheme = authenticationScheme.Name
                    }
                ).ToList();

            var allowLocal = true;

            if (context?.Client.ClientId != null)
            {
                var client = await Clients.FindEnabledClientByIdAsync(context.Client.ClientId);

                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var viewModel = await BuildLoginViewModelAsync(model.ReturnUrl);

            viewModel.Email = model.Email;
            viewModel.RememberLogin = model.RememberLogin;

            return viewModel;
        }
    }
}
