using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Models;
using Microsoft.AspNetCore.Identity;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class PasswordValidator : IPasswordValidator
    {
        private IUserStore<User> Users { get; }

        private IPasswordHasher<User> PasswordHasher { get; }
        
        public PasswordValidator(IUserStore<User> users, IPasswordHasher<User> passwordHasher)
            : base()
        {
            Users = users;
            PasswordHasher = passwordHasher;
        }

        public async Task<bool> ValidateCredentialsAsync(string userName, string password)
        {
            return await Task.Run<bool>
            (
                async () =>
                {
                    var user = await Users.FindByNameAsync(userName, CancellationToken.None);
            
                    if (user == null)
                    {
                        return false;
                    }

                    var result = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                    if (result == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        // TODO : rehash password
                    }
            
                    return result != PasswordVerificationResult.Failed;
                }
            );
        }
    }
}
