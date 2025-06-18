using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToValidApiEndpoint(this Dictionary<string, string> input)
        {
            var protocol = "https";
            if (input.TryGetValue("https", out var isHttps) && isHttps == "false")
            {
                protocol = "http";
            }

            return $"{protocol}://{input["endpoint"]}/{input["Host"]}{input["Target"]}";
        }
    }
}