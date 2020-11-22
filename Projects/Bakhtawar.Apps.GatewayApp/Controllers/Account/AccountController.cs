using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Apps.GatewayApp.Models.Account;
using Bakhtawar.Apps.GatewayApp.ViewModels.Account;
using Bakhtawar.Models;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private IAuthenticationSchemeProvider SchemeProvider { get; }
        
        private ILookupNormalizer KeyNormalizer { get; }

        private IPasswordValidator PasswordValidator { get; }
        
        private IEmailSender EmailSender { get; }
        
        private SignInManager<User> SignInManager { get; }
        
        private UserManager<User> UserManager { get; }

        private IIdentityServerInteractionService Interaction { get; }

        private IEventService Events { get; }

        private IClientStore Clients { get; }

        private IUserStore<User> UserStore { get; }
        
        private IUserEmailStore<User> UserEmailStore { get; }

        public AccountController
        (
            IAuthenticationSchemeProvider schemeProvider,
            ILookupNormalizer keyNormalizer,
            IPasswordValidator passwordValidator,
            IEmailSender emailSender,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            IClientStore clients,
            IUserStore<User> userStore,
            IUserEmailStore<User> userEmailStore
        )
        {
            SchemeProvider = schemeProvider;
            KeyNormalizer = keyNormalizer;
            PasswordValidator = passwordValidator;
            EmailSender = emailSender;
            SignInManager = signInManager;
            UserManager = userManager;
            Interaction = interaction;
            Events = events;
            Clients = clients;
            UserStore = userStore;
            UserEmailStore = userEmailStore;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var visibleExternalProviders = await GetExternalLoginsAsync(returnUrl);

            ViewData["returnUrl"] = returnUrl;
            ViewData["visibleExternalProviders"] = visibleExternalProviders;
            
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel input, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var visibleExternalProviders = await GetExternalLoginsAsync(returnUrl);

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
                        values: new { userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme
                    );

                    await EmailSender.SendEmailAsync(input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (UserManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", "Account", new { email = input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["returnUrl"] = returnUrl;
            ViewData["visibleExternalProviders"] = visibleExternalProviders;

            return View("Register", input);
        }
        
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var viewModel = await BuildLoginViewModelAsync(returnUrl); 

            if (viewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { scheme = viewModel.ExternalLoginScheme, returnUrl });
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
                if (await PasswordValidator.ValidateCredentialsAsync(model.Username, model.Password))
                {
                    var user = await UserStore.FindByNameAsync(model.Username, CancellationToken.None);

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

                await Events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId:context?.Client.ClientId));

                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var viewModel = await BuildLoginViewModelAsync(model);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var viewModel = await BuildLogoutViewModelAsync(logoutId);

            if (viewModel.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly
                return await Logout(viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var viewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await UserEmailStore.FindByEmailAsync(KeyNormalizer.NormalizeEmail(email), CancellationToken.None);

            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            ViewData["Email"] = email;

            // TODO : remove this code that confirms the account
            var displayConfirmAccountLink = true;

            ViewData["DisplayConfirmAccountLink"] = displayConfirmAccountLink;

            if (displayConfirmAccountLink)
            {
                var userId = await UserManager.GetUserIdAsync(user);
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                
                var emailConfirmationUrl = Url.Action
                (
                    "ConfirmEmail",
                    "Account",
                    new { userId, code, returnUrl },
                    protocol: Request.Scheme
                );

                ViewData["EmailConfirmationUrl"] = emailConfirmationUrl;
            }

            return View("RegisterConfirmation");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await UserManager.ConfirmEmailAsync(user, code);

            TempData["StatusMessage"] = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            return RedirectToAction("Login", "Account");
        }

        private async Task<IEnumerable<ExternalProvider>> GetExternalLoginsAsync(string returnUrl)
        {
            var externalProviders = default(IEnumerable<ExternalProvider>);

            var context = await Interaction.GetAuthorizationContextAsync(returnUrl);

            if (context?.IdP != null && await SchemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var enableLocalLogin = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                externalProviders = !enableLocalLogin
                    ? new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                    : new ExternalProvider[] { };
            }

            externalProviders = (await SchemeProvider.GetAllSchemesAsync())
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
                        externalProviders = externalProviders.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return externalProviders.ToArray();
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
                    Username = context?.LoginHint,
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
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var viewModel = await BuildLoginViewModelAsync(model.ReturnUrl);

            viewModel.Username = model.Username;
            viewModel.RememberLogin = model.RememberLogin;

            return viewModel;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var viewModel = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
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
