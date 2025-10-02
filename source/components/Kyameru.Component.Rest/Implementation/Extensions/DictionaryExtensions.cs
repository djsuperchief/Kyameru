using System.Collections.Generic;
using System.Linq;

namespace Kyameru.Component.Rest.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToValidApiEndpoint(this Dictionary<string, string> input, string[]? headersToRemove = null)
        {
            var protocol = "https";
            if (input.TryGetValue("https", out var isHttps) && isHttps == "false")
            {
                protocol = "http";
            }
            
            var port = input.TryGetValue("Port", out var portString) ? portString : "80";

            var response = $"{protocol}://{input["Host"]}:{port}{input["Target"]}";
            if (headersToRemove == null) return response;
            
            var remaining = input.Where(x => !headersToRemove.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            if (remaining.Count <= 0) return response;

            response += "?";
            foreach (var key in remaining.Keys)
            {
                response += $"{key}={remaining[key]}&";
            }

            // Remove last ampersand
            response = response.Substring(0, response.Length - 1);

            return response;
        }
    }
}