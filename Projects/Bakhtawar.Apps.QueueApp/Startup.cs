using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bakhtawar.Apps.QueueApp.Filters;
using Bakhtawar.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Bakhtawar.Apps.QueueApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
            : base()
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                services.AddCors
                (
                    (options) =>
                    {
                        options.AddDefaultPolicy
                        (
                            (builder) =>
                            {
                                builder
                                    .WithOrigins(Configuration["Cors:Origins"])
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials();
                            }
                        );
                    }
                );
            }
            
            services
                .AddControllers
                (
                    (options) =>
                    {
                        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                    }
                );

            services
                .AddAuthentication("Bearer")
                .AddJwtBearer
                (
                    "Bearer",
                    (options) =>
                    {
                        options.Authority = Configuration["JwtBearer:Authority"];
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                        options.RequireHttpsMetadata = false;
                    }
                );

            services
                .AddAuthorization
                (
                    (options) =>
                    {
                        options.AddPolicy
                        (
                            "SpaScope",
                            (policy) =>
                            {
                                policy.RequireAuthenticatedUser();
                                policy.RequireClaim("scope", "openid", "bakhtawar.web");
                            }
                        );
                    }
                );

            services
                .AddHangfire
                (
                    (configuration) =>
                    {
                        configuration
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseColouredConsoleLogProvider()
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UsePostgreSqlStorage
                            (
                                Configuration["ConnectionStrings:Bakhtawar"]
                            );
                    }
                );

            services
                .Configure<ForwardedHeadersOptions>
                (
                    (options) =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                        if (Environment.IsDevelopment())
                        {
                            return;
                        }

                        var knownNetworks = Configuration["ForwardedHeadersOptions:KnownNetworks"];

                        if (string.IsNullOrEmpty(knownNetworks))
                        {
                            return;
                        }

                        foreach (var knownNetwork in knownNetworks.Split(";"))
                        {
                            var parts = knownNetwork.Split(":");

                            var prefix = parts[0];
                            var prefixLength = int.Parse(parts[1]);

                            options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(prefix), prefixLength));
                        }
                    }
                );
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHangfireServer();
            app.UseHangfireDashboard
            (
                "/.well-known/hangfire-dashboard",
                new DashboardOptions
                {
                    Authorization = new []
                    {
                        new HardPassAuthorizationFilter()
                    }
                }
            );

            app.UseEndpoints
            (
                (endpoints) =>
                {
                    endpoints
                        .MapControllers()
                        .RequireAuthorization("SpaScope");
                }
            );
        }
    }
}
