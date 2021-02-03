using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Models;
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
    public partial class AccountController : Controller
    {
        private IAuthenticationSchemeProvider SchemeProvider { get; }
        
        private ILookupNormalizer KeyNormalizer { get; }

        private IPasswordValidator PasswordValidator { get; }
        
        private IUserProvisioner<User> UserProvisioner { get; }
        
        private IEmailSender EmailSender { get; }
        
        private SignInManager<User> SignInManager { get; }
        
        private UserManager<User> UserManager { get; }

        private IIdentityServerInteractionService Interaction { get; }

        private IEventService Events { get; }

        private IClientStore Clients { get; }

        private IUserStore<User> UserStore { get; }
        
        private IUserLoginStore<User> UserLoginStore { get; }

        private IUserEmailStore<User> UserEmailStore { get; }

        public AccountController
        (
            IAuthenticationSchemeProvider schemeProvider,
            ILookupNormalizer keyNormalizer,
            IPasswordValidator passwordValidator,
            IUserProvisioner<User> userProvisioner,
            IEmailSender emailSender,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            IClientStore clients,
            IUserStore<User> userStore,
            IUserLoginStore<User> userLoginStore,
            IUserEmailStore<User> userEmailStore
        )
        {
            SchemeProvider = schemeProvider;
            KeyNormalizer = keyNormalizer;
            PasswordValidator = passwordValidator;
            UserProvisioner = userProvisioner;
            EmailSender = emailSender;
            SignInManager = signInManager;
            UserManager = userManager;
            Interaction = interaction;
            Events = events;
            Clients = clients;
            UserStore = userStore;
            UserLoginStore = userLoginStore;
            UserEmailStore = userEmailStore;
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
    }
}
