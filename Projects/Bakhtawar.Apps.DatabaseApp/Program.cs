using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bakhtawar.Contracts;
using Bakhtawar.Data;
using Bakhtawar.Services;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bakhtawar.Apps.DatabaseApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            
            var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            await dataSeeder.SeedDataAsync(configuration);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var connectionString = context.Configuration[$"ConnectionStrings:Bakhtawar"]; 

            services.AddOperationalDbContext
            (
                (options) =>
                {
                    options.ConfigureDbContext = (db) => db
                        .UseNpgsql
                        (
                            connectionString,
                            (sql) => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)
                        );
                }
            );

            services.AddConfigurationDbContext
            (
                (options) =>
                {
                    options.ConfigureDbContext = (db) => db
                        .UseNpgsql
                        (
                            connectionString,
                            (sql) => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)
                        );
                }
            );

            services.AddDbContext<DataDbContext>
            (
                (db) => db
                    .UseNpgsql
                    (
                        connectionString,
                        (sql) => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)
                    )
            );

            services.AddDbContext<KeysDbContext>
            (
                (db) => db
                    .UseNpgsql
                    (
                        connectionString,
                        (sql) => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)
                    )
            );

            services
                .AddScoped<IDataSeeder, DataSeeder>();
        }
    }
}
