using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToValidApiEndpoint(this Dictionary<string, string> input, string[] headersToRemove = null)
        {
            var protocol = "https";
            if (input.TryGetValue("https", out var isHttps) && isHttps == "false")
            {
                protocol = "http";
            }

            var response = $"{protocol}://{input["endpoint"]}/{input["Host"]}{input["Target"]}";
            if (headersToRemove != null)
            {
                var remaining = input.Where(x => !headersToRemove.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                if (remaining.Count <= 0) return response;

                response += "?";
                foreach (var key in remaining.Keys)
                {
                    response += $"{key}={remaining[key]}&";
                }

                // Remove last ampersand
                response = response.Substring(0, response.Length - 1);
            }

            return response;
        }
    }
}