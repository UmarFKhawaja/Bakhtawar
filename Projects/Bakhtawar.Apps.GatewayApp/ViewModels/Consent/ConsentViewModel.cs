﻿using System.Collections.Generic;
using Bakhtawar.Apps.GatewayApp.Models.Consent;

namespace Bakhtawar.Apps.GatewayApp.ViewModels.Consent
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientName { get; set; }

        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }

        public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}
