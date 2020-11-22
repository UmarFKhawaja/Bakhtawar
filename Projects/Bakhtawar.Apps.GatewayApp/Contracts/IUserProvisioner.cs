using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Models;
using Microsoft.AspNetCore.Identity;

namespace Bakhtawar.Apps.GatewayApp.Contracts
{
    public interface IUserProvisioner<TUser>
        where TUser : IdentityUser
    {
        Task<TUser> ProvisionUserAsync(string loginProvider, string providerKey, IEnumerable<Claim> claims, CancellationToken cancellationToken);
    }
}