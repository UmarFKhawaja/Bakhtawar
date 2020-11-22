using System;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class DoNothingCorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(true);
        }
    }
}
