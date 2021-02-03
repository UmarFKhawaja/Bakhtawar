using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Models.Account;
using Bakhtawar.Apps.GatewayApp.ViewModels.Account;
using Bakhtawar.Models;
using IdentityModel;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Account
{
    public partial class AccountController
    {
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var viewModel = await BuildRegisterViewModelAsync(returnUrl);

            if (viewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "Account", new { scheme = viewModel.ExternalLoginScheme, returnUrl });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel input)
        {
            if (ModelState.IsValid)
            {
                var id = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

                var normalizedEmail = KeyNormalizer.NormalizeEmail(input.Email);

                var user = await UserStore.FindByNameAsync(normalizedEmail, CancellationToken.None) ?? new User
                {
                    Id = id,
                    UserName = input.Email,
                    Email = input.Email,
                    NormalizedEmail = normalizedEmail
                };

                var result = default(IdentityResult);

                if (user.Id == id)
                {
                    result = await UserManager.CreateAsync(user, input.Password);
                }
                else
                {
                    result = IdentityResult.Failed(new IdentityError
                    {
                        Code = "USER_NOT_CREATED",
                        Description = "Please login with your existing account; user already exists"
                    });
                }

                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page
                    (
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code, returnUrl = input.ReturnUrl },
                        protocol: Request.Scheme
                    );

                    await EmailSender.SendEmailAsync(input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (UserManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", "Account", new { email = input.Email, returnUrl = input.ReturnUrl });
                    }
                    else
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect(input.ReturnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(input);
        }

        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl)
        {
            var context = await Interaction.GetAuthorizationContextAsync(returnUrl);

            if (context?.IdP != null && await SchemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var viewModel = new RegisterViewModel
                {
                    EnableLocalLogin = local,
                    Email = context?.LoginHint,
                    ReturnUrl = returnUrl,
                    ExternalProviders = !local
                        ? new[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                        : Enumerable.Empty<ExternalProvider>()
                };

                return viewModel;
            }

            var schemes = await SchemeProvider.GetAllSchemesAsync();

            var externalProviders = schemes
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
                        externalProviders = externalProviders
                            .Where
                            (
                                (externalProvider) => client.IdentityProviderRestrictions.Contains(externalProvider.AuthenticationScheme)
                            )
                            .ToList();
                    }
                }
            }

            return new RegisterViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                Email = context?.LoginHint,
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders.ToArray()
            };
        }
    }
}
