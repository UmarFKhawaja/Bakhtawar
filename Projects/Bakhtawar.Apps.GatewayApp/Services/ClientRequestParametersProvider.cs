using System;
using System.Collections.Generic;
using System.Linq;
using Bakhtawar.Apps.GatewayApp.Contracts;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class ClientRequestParametersProvider : IClientRequestParametersProvider
    {
        public ClientRequestParametersProvider(ConfigurationDbContext configurationDbContext, IAbsoluteUrlGenerator urlGenerator)
            : base()
        {
            ConfigurationDbContext = configurationDbContext;
            UrlGenerator = urlGenerator;
        }

        private ConfigurationDbContext ConfigurationDbContext { get; }
        
        private IAbsoluteUrlGenerator UrlGenerator { get; }

        public IDictionary<string, string> GetClientParameters(HttpContext context, string clientId)
        {
            var client = ConfigurationDbContext.Clients
                .Include((c) => c.RedirectUris)
                .Include((c) => c.PostLogoutRedirectUris)
                .Include((c) => c.AllowedScopes)
                .Single((c) => c.ClientId == clientId);

            var authority = context.GetIdentityServerIssuerUri();
            var responseType = "code";

            var redirectUri = UrlGenerator.GenerateAbsoluteUrl(context, client.RedirectUris.First().RedirectUri);
            var postLogoutRedirectUri = UrlGenerator.GenerateAbsoluteUrl(context, client.PostLogoutRedirectUris.First().PostLogoutRedirectUri);
            var scopes = string.Join(" ", client.AllowedScopes.Select((s) => s.Scope));

            return new Dictionary<string, string>
            {
                ["authority"] = authority,
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["post_logout_redirect_uri"] = postLogoutRedirectUri,
                ["response_type"] = responseType,
                ["scope"] = scopes
            };
        }
    }
}