using System;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Extensions
{
    public static class RouteAttributesExtensions
    {
        public static string ToValidApiEndpoint(this RouteAttributes input)
        {
            var protocol = "https";
            if (input.Headers.TryGetValue("https", out var isHttps) && isHttps == "false")
            {
                protocol = "http";
            }

            return $"{protocol}://{input.Headers["endpoint"]}/{input.Headers["Host"]}{input.Headers["Target"]}";
        }
    }
}