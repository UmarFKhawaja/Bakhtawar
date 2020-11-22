using System;
using Microsoft.AspNetCore.Http;

namespace Bakhtawar.Apps.GatewayApp.Contracts
{
    public interface IAbsoluteUrlGenerator
    {
        string GenerateAbsoluteUrl(string path);

        string GenerateAbsoluteUrl(HttpContext context, string path);
    }
}
