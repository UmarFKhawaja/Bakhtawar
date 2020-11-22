using System;
using Bakhtawar.Apps.GatewayApp.Contracts;
using Microsoft.AspNetCore.Http;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class AbsoluteUrlGenerator : IAbsoluteUrlGenerator
    {
        public AbsoluteUrlGenerator(IHttpContextAccessor httpContextAccessor)
        {
            // We need the context accessor here in order to produce an absolute url from a potentially relative url.
            ContextAccessor = httpContextAccessor;
        }

        private IHttpContextAccessor ContextAccessor { get; }

        // Call this method when you are overriding a service that doesn't have an HttpContext instance available.
        public string GenerateAbsoluteUrl(string path)
        {
            var (process, result) = ShouldProcessPath(path);
            if (!process)
            {
                return result;
            }

            if (ContextAccessor.HttpContext?.Request == null)
            {
                throw new InvalidOperationException("The request is not currently available. This service can only be used within the context of an existing HTTP request.");
            }

            return GenerateAbsoluteUrl(ContextAccessor.HttpContext, path);
        }

        // Call this method when you are implementing a service that has an HttpContext instance available.
        public string GenerateAbsoluteUrl(HttpContext context, string path)
        {
            var (process, result) = ShouldProcessPath(path);
            if (!process)
            {
                return result;
            }
            var request = context.Request;
            return $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}{path}";
        }

        private (bool, string) ShouldProcessPath(string path)
        {
            if (path == null || !Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                return (false, null);
            }

            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return (false, path);
            }

            return (true, path);
        }
    }
}