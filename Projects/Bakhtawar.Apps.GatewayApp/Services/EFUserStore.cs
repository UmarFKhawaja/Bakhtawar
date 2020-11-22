using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Data;
using Bakhtawar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class EFUserStore : IUserStore<User>, IUserPasswordStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>, IUserLoginStore<User>, IUserLockoutStore<User>
    {
        public EFUserStore(IServiceProvider serviceProvider)
            : base()
        {
            ServiceProvider = serviceProvider;
        }
        
        private IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            await dbContext.Set<User>().AddAsync(user, cancellationToken);
            
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            
            var count = await dbContext.SaveChangesAsync(cancellationToken);

            var result = count > 0
                ? IdentityResult.Success
                : IdentityResult.Failed
                (
                    new IdentityError
                    {
                        Code = "USER_NOT_CREATED",
                        Description = $"user {user.Email} could not be created"
                    }
                );

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Set<User>().Attach(user);

            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            
            var count = await dbContext.SaveChangesAsync(cancellationToken);

            var result = count > 0
                ? IdentityResult.Success
                : IdentityResult.Failed
                (
                    new IdentityError
                    {
                        Code = "USER_NOT_UPDATED",
                        Description = $"user {user.Email} could not be updated"
                    }
                );

            return result;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Set<User>().Remove(user);
            
            var count = await dbContext.SaveChangesAsync(cancellationToken);

            var result = count > 0
                ? IdentityResult.Success
                : IdentityResult.Failed
                (
                    new IdentityError
                    {
                        Code = "USER_NOT_DELETED",
                        Description = $"user {user.Email} could not be deleted"
                    }
                );

            return result;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var user = await dbContext.Set<User>().FirstOrDefaultAsync((u) => u.Id == userId, cancellationToken);

            return user;
        }

        public async Task<User> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var normalizedUserName = userName.ToUpperInvariant();

            var user = await dbContext.Set<User>().FirstOrDefaultAsync((u) => u.NormalizedUserName == normalizedUserName, cancellationToken);

            return user;
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.Run(() => user.Id, cancellationToken);
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.Run(() => user.UserName, cancellationToken);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.CompletedTask;
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.Run(() => user.NormalizedUserName, cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedUserName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedUserName;

            return Task.CompletedTask;
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public async Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var keyNormalizer = scope.ServiceProvider.GetService<ILookupNormalizer>();

            var normalizedEmail = keyNormalizer.NormalizeEmail(email);

            var user = await dbContext.Set<User>().FirstOrDefaultAsync((u) => u.NormalizedEmail == normalizedEmail, cancellationToken);

            return user;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;

            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool emailConfirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = emailConfirmed;

            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool phoneNumberConfirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = phoneNumberConfirmed;

            return Task.CompletedTask;
        }

        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.UserLogins.Add
            (
                new UserLogin
                {
                    User = user,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName
                }
            );
            
            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            user.UserLogins.Remove
            (
                user.UserLogins.First
                (
                    (userLogin) =>
                    userLogin.LoginProvider == loginProvider
                    &&
                    userLogin.ProviderKey == providerKey
                )
            );

            return Task.CompletedTask;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var userLoginInfos = await dbContext
                .UserLogins
                .Where((userLogin => userLogin.UserId == user.Id))
                .Select
                (
                    (userLogin) => new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey, userLogin.ProviderDisplayName)
                )
                .ToListAsync(cancellationToken);

            return userLoginInfos;
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var user = await dbContext
                .UserLogins
                .Where((userLogin) => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey)
                .Select((userLogin) => userLogin.User)
                .SingleOrDefaultAsync(cancellationToken);

            return user;
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount += 1;
            
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(User user, bool lockoutEnabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = lockoutEnabled;

            return Task.CompletedTask;
        }
    }
}
