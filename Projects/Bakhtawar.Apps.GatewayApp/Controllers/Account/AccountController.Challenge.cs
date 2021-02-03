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
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && Interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)), 
                Items =
                {
                    { "returnUrl", returnUrl }, 
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
        }
    }
}
