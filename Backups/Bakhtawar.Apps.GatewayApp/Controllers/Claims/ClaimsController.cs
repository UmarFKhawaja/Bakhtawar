using System.Linq;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Filters;
using Bakhtawar.Apps.GatewayApp.ViewModels.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bakhtawar.Apps.GatewayApp.Controllers.Claims
{
    [SecurityHeaders]
    [Authorize]
    public class ClaimsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };

            var isLocallyRequested = !localAddresses.Contains(HttpContext.Connection.RemoteIpAddress.ToString());

            var viewModel = new ClaimsViewModel(await HttpContext.AuthenticateAsync(), isLocallyRequested);

            return View(viewModel);
        }
    }
}