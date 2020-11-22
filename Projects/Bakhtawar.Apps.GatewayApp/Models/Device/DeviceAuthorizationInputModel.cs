using Bakhtawar.Apps.GatewayApp.Models.Consent;

namespace Bakhtawar.Apps.GatewayApp.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}
