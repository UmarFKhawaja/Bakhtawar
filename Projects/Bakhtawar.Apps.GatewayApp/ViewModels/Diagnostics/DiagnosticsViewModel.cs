using System.Collections.Generic;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace Bakhtawar.Apps.GatewayApp.ViewModels.Diagnostics
{
    public class DiagnosticsViewModel
    {
        public DiagnosticsViewModel(AuthenticateResult authenticateResult, bool isLocallyRequested)
        {
            AuthenticateResult = authenticateResult;

            if (authenticateResult.Properties.Items.ContainsKey("client_list"))
            {
                var encoded = authenticateResult.Properties.Items["client_list"];
                var bytes = Base64Url.Decode(encoded);
                var value = Encoding.UTF8.GetString(bytes);

                Clients = JsonConvert.DeserializeObject<string[]>(value);
            }

            IsLocallyRequested = isLocallyRequested;
        }
        
        public bool IsLocallyRequested { get; }
        
        public AuthenticateResult AuthenticateResult { get; }

        public IEnumerable<string> Clients { get; } = new List<string>();
    }
}
