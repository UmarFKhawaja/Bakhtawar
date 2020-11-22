using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class UserProvisioner : IUserProvisioner<User>
    {
        private IUserStore<User> UserStore { get; }

        private IUserEmailStore<User> UserEmailStore { get; }

        private IUserLoginStore<User> UserLoginStore { get; }
        
        private ILookupNormalizer LookupNormalizer { get; }

        public UserProvisioner(IUserStore<User> userStore, IUserEmailStore<User> userEmailStore, IUserLoginStore<User> userLoginStore, ILookupNormalizer lookupNormalizer)
            : base()
        {
            UserStore = userStore;
            UserEmailStore = userEmailStore;
            UserLoginStore = userLoginStore;
            LookupNormalizer = lookupNormalizer;
        }

        public async Task<User> ProvisionUserAsync(string loginProvider, string providerKey, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (filtered.All(x => x.Type != JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;

                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }
            
            // create a new unique subject id
            var id = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

            // check if a display name is available, otherwise fallback to subject id
            var displayName= filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? id;

            // get email, or cause exception if not present
            var email= filtered.First(c => c.Type == JwtClaimTypes.Email).Value;

            var normalizedEmail = LookupNormalizer.NormalizeEmail(email);

            // create new user
            var user = await UserEmailStore.FindByEmailAsync(email, cancellationToken) ?? await UserLoginStore.FindByLoginAsync(loginProvider, providerKey, cancellationToken) ?? new User
            {
                Id = id,
                UserName = email,
                NormalizedUserName = normalizedEmail,
                Email = email,
                NormalizedEmail = normalizedEmail
            };
            
            user.UserLogins.Add
            (
                new UserLogin
                {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    ProviderDisplayName = displayName,
                    User = user
                }
            );

            user.UserClaims = user.UserClaims.ToArray()
                .Union
                (
                    filtered
                        .Select
                        (
                            (claim) => new UserClaim
                            {
                                ClaimType = claim.Type,
                                ClaimValue = claim.Value,
                                User = user
                            }
                        ).ToArray(),
                    EqualityComparer<UserClaim>.Default
                )
                .ToHashSet();

            if (user.Id == id)
            {
                // add user to store
                await UserStore.CreateAsync(user, cancellationToken);
            }
            else
            {
                // update user in store
                await UserStore.UpdateAsync(user, cancellationToken);
            }

            return user;
        }
    }
}