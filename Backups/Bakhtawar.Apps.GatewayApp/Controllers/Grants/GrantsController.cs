using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Apps.GatewayApp.ViewModels.Grants;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Grants
{
    [SecurityHeaders]
    [Authorize]
    public class GrantsController : Controller
    {
        private IIdentityServerInteractionService Interaction { get; }

        private IClientStore Clients { get; }

        private IResourceStore Resources { get; }

        private IEventService Events { get; }

        public GrantsController(IIdentityServerInteractionService interaction, IClientStore clients, IResourceStore resources, IEventService events)
        {
            Interaction = interaction;
            Clients = clients;
            Resources = resources;
            Events = events;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await BuildViewModelAsync();

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await Interaction.RevokeUserConsentAsync(clientId);
            await Events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));

            return RedirectToAction("Index");
        }

        private async Task<GrantsViewModel> BuildViewModelAsync()
        {
            var grants = await Interaction.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();

            foreach (var grant in grants)
            {
                var client = await Clients.FindClientByIdAsync(grant.ClientId);

                if (client != null)
                {
                    var resources = await Resources.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel()
                    {
                        ClientId = client.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        ClientLogoUrl = client.LogoUri,
                        ClientUrl = client.ClientUri,
                        Description = grant.Description,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return new GrantsViewModel
            {
                Grants = list
            };
        }
    }
}