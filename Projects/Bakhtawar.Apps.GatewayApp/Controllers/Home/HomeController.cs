using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Apps.GatewayApp.ViewModels.Shared;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Home
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        protected IIdentityServerInteractionService Interaction { get; }

        protected IWebHostEnvironment Environment { get; }

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
        {
            Interaction = interaction;
            Environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var viewModel = new ErrorViewModel();

            var message = await Interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                viewModel.Error = message;

                if (!Environment.IsDevelopment())
                {
                    message.ErrorDescription = null;
                }
            }

            return View("Error", viewModel);
        }
    }
}
