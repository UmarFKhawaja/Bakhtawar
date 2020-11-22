using System;
using System.Threading;
using System.Threading.Tasks;
using Bakhtawar.Data;
using Bakhtawar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class EFRoleStore : IRoleStore<Role>
    {
        public EFRoleStore(IServiceProvider serviceProvider)
            : base()
        {
            ServiceProvider = serviceProvider;
        }
        
        private IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            await dbContext.Set<Role>().AddAsync(role, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            var result = IdentityResult.Success;

            return result;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Set<Role>().Attach(role);

            await dbContext.SaveChangesAsync(cancellationToken);

            var result = IdentityResult.Success;

            return result;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Remove(role);

            var result = IdentityResult.Success;

            return result;
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var role = await dbContext.Set<Role>().FirstOrDefaultAsync((r) => r.Id == roleId, cancellationToken);

            return role;
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            var role = await dbContext.Set<Role>().FirstOrDefaultAsync((r) => r.NormalizedName == normalizedRoleName, cancellationToken);

            return role;
        }

        public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.Run(() => role.Id, cancellationToken);
        }

        public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.Run(() => role.Name, cancellationToken);
        }

        public async Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Set<Role>().Attach(role);

            role.Name = roleName;

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.Run(() => role.NormalizedName, cancellationToken);
        }

        public async Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            var scope = ServiceProvider.CreateScope();

            await using var dbContext = scope.ServiceProvider.GetService<DataDbContext>();

            dbContext.Set<Role>().Attach(role);

            role.NormalizedName = normalizedName;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}