using Bakhtawar.Apps.GatewayApp.ViewModels.Account;
using IdentityServer4.Models;

namespace Bakhtawar.Apps.GatewayApp.Results
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri != null;

        public string RedirectUri { get; set; }
        
        public Client Client { get; set; }

        public bool ShowView => ViewModel != null;
        
        public ConsentViewModel ViewModel { get; set; }

        public bool HasValidationError => ValidationError != null;
        
        public string ValidationError { get; set; }
    }
}
