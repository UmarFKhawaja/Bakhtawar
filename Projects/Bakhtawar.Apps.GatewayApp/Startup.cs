using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Bakhtawar.Apps.GatewayApp.Services;
using Bakhtawar.Data;
using Bakhtawar.Models;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slugify;

namespace Bakhtawar.Apps.GatewayApp
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
                    (builder) => builder.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"], b => b.MigrationsAssembly("Bakhtawar.Apps.GatewayApp"))
                );

            services
                .AddDbContext<KeysDbContext>
                (
                    (builder) => builder.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"], b => b.MigrationsAssembly("Bakhtawar.Apps.GatewayApp"))
                );
            
            services
                .AddSingleton<EFUserStore>()
                .AddSingleton<EFRoleStore>()
                .AddSingleton<IUserStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IUserEmailStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IUserPhoneNumberStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IUserPasswordStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IUserLoginStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IUserLockoutStore<User>>((serviceProvider) => serviceProvider.GetService<EFUserStore>())
                .AddSingleton<IRoleStore<Role>>((serviceProvider) => serviceProvider.GetService<EFRoleStore>());

            services
                .AddIdentity<User, Role>
                (
                    (options) =>
                    {
                        options.SignIn.RequireConfirmedAccount = true;
                        options.Password.RequireNonAlphanumeric = false;
                    }
                )
                .AddDefaultTokenProviders();

            services
                .AddIdentityServer
                (
                    (options) =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                        
                        // HINT : see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                        options.EmitStaticAudienceClaim = true;
                    }
                )
                .AddAspNetIdentity<User>()
                // NOTE : adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore
                (
                    (options) =>
                    {
                        options.ConfigureDbContext = (builder) => builder.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"], b => b.MigrationsAssembly("Bakhtawar.Apps.GatewayApp"));
                    }
                )
                // NOTE : adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore
                (
                    (options) =>
                    {
                        options.ConfigureDbContext = (builder) => builder.UseNpgsql(Configuration["ConnectionStrings:Bakhtawar"], b => b.MigrationsAssembly("Bakhtawar.Apps.GatewayApp"));

                        // NOTE : enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                    }
                )
                .AddServices
                (
                    Environment.IsDevelopment(),
                    (builder) => builder.AddDeveloperSigningCredential(false),
                    (builder) => builder.AddSigningCredential
                    (
                        new X509Certificate2
                        (
                            File.ReadAllBytes(Configuration["IdentityServer:Key:FilePath"]),
                            (string) Configuration["IdentityServer:Key:Password"]
                        )
                    )
                );

            services
                .AddDataProtection()
                .PersistKeysToDbContext<KeysDbContext>()
                .SetApplicationName(Configuration["Application:Name"]);

            services
                .AddAuthentication()
                .AddCookie("Cookies")
                .AddService
                (
                    Configuration["Secret:Google:ClientId"] != null && Configuration["Secret:Google:ClientSecret"] != null,
                    (builder) => builder
                        .AddGoogle
                        (
                            "Google",
                            (options) =>
                            {
                                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                                // NOTE : register your IdentityServer with Google at https://console.developers.google.com
                                //        enable the Google+ API
                                //        set the redirect URI to https://localhost:4443/signin-google & https://id.bakhtawar.co.uk/signin-google
                                options.ClientId = Configuration["Secret:Google:ClientId"];
                                options.ClientSecret = Configuration["Secret:Google:ClientSecret"];
                            }
                        )
                );

            services
                .AddOidcStateDataFormatterCache();

            services
                .AddControllersWithViews();

            services
                .AddRazorPages();
            
            services
                .AddSameSiteCookiePolicy();

            services
                .AddCors
                (
                    (options) =>
                    {
                        options.AddPolicy
                        (
                            "api",
                            (policy) =>
                            {
                                policy
                                    .AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            }
                        );
                    }
                );

            services
                .AddSingleton<IEmailSender, FileSystemEmailSender>()
                .AddSingleton<SlugHelper>()
                .AddScoped<IUserProvisioner<User>, UserProvisioner>()
                .AddScoped<IClientRequestParametersProvider, ClientRequestParametersProvider>()
                .AddScoped<IAbsoluteUrlGenerator, AbsoluteUrlGenerator>()
                .AddTransient<IPasswordValidator, PasswordValidator>()
                .AddTransient<IRedirectUriValidator, DoNothingRedirectValidator>()
                .AddTransient<ICorsPolicyService, DoNothingCorsPolicyService>();

            services.Configure<ForwardedHeadersOptions>
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseCors("api");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints
            (
                (endpoints) =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                }
            );
        }
    }

    public static class SameSiteCookiePolicyExtensions
    {
        public static IServiceCollection AddSameSiteCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>
            (
                (options) =>
                {
                    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                    options.OnAppendCookie = (cookieContext) => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                    options.OnDeleteCookie = (cookieContext) => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                }
            );

            return services;
        }

        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

                if (DisallowsSameSiteNone(userAgent))
                {
                    // NOTE : for .NET Core < 3.1, set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        private static bool DisallowsSameSiteNone(string userAgent)
        {
            // NOTE : cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking stack
            if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // NOTE : cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // NOTE : cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }
    }

    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddService(this AuthenticationBuilder builder, bool condition, Func<AuthenticationBuilder, AuthenticationBuilder> build)
        {
            if (condition)
            {
                builder = build(builder);
            }
            
            return builder;
        }

        public static AuthenticationBuilder AddService(this AuthenticationBuilder builder, Func<bool> condition, Func<AuthenticationBuilder, AuthenticationBuilder> build)
        {
            return builder.AddService(condition(), build);
        }
    }

    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddServices
        (
            this IIdentityServerBuilder identityServerBuilder,
            Func<bool> condition,
            Func<IIdentityServerBuilder, IIdentityServerBuilder> ifTrue,
            Func<IIdentityServerBuilder, IIdentityServerBuilder> ifFalse
        )
        {
            return identityServerBuilder.AddServices(condition(), ifTrue, ifFalse);
        }
        
        public static IIdentityServerBuilder AddServices
        (
            this IIdentityServerBuilder identityServerBuilder,
            bool condition,
            Func<IIdentityServerBuilder, IIdentityServerBuilder> ifTrue,
            Func<IIdentityServerBuilder, IIdentityServerBuilder> ifFalse
        )
        {
            if (condition)
            {
                return ifTrue(identityServerBuilder);
            }
            else
            {
                return ifFalse(identityServerBuilder);
            }
        }
    }
}
