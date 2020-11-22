using Bakhtawar.Apps.GatewayApp.ViewModels.Consent;

namespace Bakhtawar.Apps.GatewayApp.ViewModels.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }

        public bool ConfirmUserCode { get; set; }
    }
}
