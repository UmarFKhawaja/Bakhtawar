using Bakhtawar.Apps.GatewayApp.Models.Account;

namespace Bakhtawar.Apps.GatewayApp.ViewModels.Account
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}
