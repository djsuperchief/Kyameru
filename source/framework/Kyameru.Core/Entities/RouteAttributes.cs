using Kyameru.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Entities
{
    public class RouteAttributes
    {
        public string ComponentName { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }

        public RouteAttributes(string componentUri)
        {
            UriBuilder uriBuilder = new UriBuilder(componentUri);
            this.ComponentName = uriBuilder.Scheme.ToFirstCaseUpper();
            this.Headers = this.ParseQuery($"Target={uriBuilder.Path}{this.GetQuery(uriBuilder)}");
        }

        private Dictionary<string, string> ParseQuery(string query)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            string[] parts = query.Split('&');
            for (int i = 0; i < parts.Length; i++)
            {
                response.Add(parts[i].Split('=')[0], parts[i].Split('=')[1]);
            }

            return response;
        }

        private string GetQuery(UriBuilder uriBuilder)
        {
            string response = string.Empty;
            if (!string.IsNullOrWhiteSpace(uriBuilder.Query))
            {
                response = $"&{uriBuilder.Query.Substring(1)}";
            }

            return response;
        }
    }
}