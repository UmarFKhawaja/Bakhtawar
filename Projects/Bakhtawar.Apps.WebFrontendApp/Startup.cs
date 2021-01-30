using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bakhtawar.Data;
using Bakhtawar.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bakhtawar.Apps.WebFrontendApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<DataDbContext>
                (
                    (options) => { options.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"]); }
                );

            services
                .AddDbContext<KeysDbContext>
                (
                    (options) => { options.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"]); }
                );

            services
                .AddDataProtection()
                .PersistKeysToDbContext<KeysDbContext>()
                .SetApplicationName(Configuration["Application:Name"]);

            services
                .AddAuthentication
                (
                    (options) =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = "oidc";
                    }
                )
                .AddCookie("Cookies")
                .AddOpenIdConnect
                (
                    "oidc",
                    (options) =>
                    {
                        options.Authority = Configuration["OIDC:Authority"];

                        options.ClientId = Configuration["OIDC:ClientId"];
                        // options.ClientSecret = Configuration["OIDC:ClientSecret"];

                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.ResponseType = OidcConstants.ResponseTypes.Code;
                        options.ResponseMode = OidcConstants.ResponseModes.Query;
                        options.UsePkce = true;

                        options.Scope.Add("openid");
                        options.Scope.Add("profile");
                        options.Scope.Add("email");
                        options.Scope.Add("bakhtawar.users");
                        options.Scope.Add("bakhtawar.galleries");
                        options.Scope.Add("bakhtawar.posts");
                        options.Scope.Add("bakhtawar.comments");
                        
                        options.RequireHttpsMetadata = true;

                        options.SaveTokens = true;
                    }
                );

            services
                .AddControllersWithViews
                (
                    (options) =>
                    {
                        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                    }
                );

            services
                .AddRazorPages();

            services
                .AddSpaStaticFiles
                (
                    (options) => { options.RootPath = "App/build"; }
                );

            services
                .Configure<ForwardedHeadersOptions>
                (
                    (options) =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;

                        if (!Environment.IsDevelopment())
                        {
                            var knownNetworks = Configuration["ForwardedHeadersOptions:KnownNetworks"];

                            if (!string.IsNullOrEmpty(knownNetworks))
                            {
                                foreach (var knownNetwork in knownNetworks.Split(";"))
                                {
                                    var parts = knownNetwork.Split(":");

                                    var prefix = parts[0];
                                    var prefixLength = int.Parse(parts[1]);

                                    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(prefix), prefixLength));
                                }
                            }
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints
            (
                (endpoints) =>
                {
                    endpoints
                        .MapDefaultControllerRoute()
                        .RequireAuthorization();
                    endpoints
                        .MapRazorPages()
                        .RequireAuthorization();
                }
            );

            app.UseSpa
            (
                (spa) =>
                {
                    spa.Options.SourcePath = "App";

                    if (Environment.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                }
            );
        }
    }

    public static class AuthenticationBuilderExtension
    {
        public static AuthenticationBuilder AddService(this AuthenticationBuilder builder, bool condition, Func<AuthenticationBuilder, AuthenticationBuilder> build)
        {
            if (condition)
            {
                builder = build(builder);
            }
            
            return builder;
        }
    }
}