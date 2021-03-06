﻿using Kyameru.Core.Extensions;
using System;
using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route attributes class.
    /// </summary>
    public class RouteAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttributes"/> class.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI.</param>
        public RouteAttributes(string componentUri)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder(componentUri);
                this.ComponentName = uriBuilder.Scheme.ToFirstCaseUpper();
                this.Headers = this.ParseQuery($"Target={uriBuilder.Path}{this.GetQuery(uriBuilder)}");
            }
            catch (Exception ex)
            {
                throw new Exceptions.RouteUriException(Resources.ERROR_ROUTE_URI, ex);
            }
        }

        /// <summary>
        /// Gets the component name.
        /// </summary>
        public string ComponentName { get; private set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Parses a URI query string.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <returns>Returns a dictionary of headers.</returns>
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

        /// <summary>
        /// Gets the query from the URI builder.
        /// </summary>
        /// <param name="uriBuilder">URI builder class.</param>
        /// <returns>Returns a URI query.</returns>
        private string GetQuery(UriBuilder uriBuilder)
        {
            string response = string.Empty;
            if (!string.IsNullOrWhiteSpace(uriBuilder.Query))
            {
                response = $"{uriBuilder.Query.Substring(1)}";
            }

            if (uriBuilder.Port != -1)
            {
                response = $"Port={uriBuilder.Port}&{response}";
            }

            if (!string.IsNullOrWhiteSpace(uriBuilder.Password))
            {
                response = $"Password={uriBuilder.Password}&{response}";
            }

            if (!string.IsNullOrWhiteSpace(uriBuilder.UserName))
            {
                response = $"UserName={uriBuilder.UserName}&{response}";
            }

            if (!string.IsNullOrWhiteSpace(uriBuilder.Host))
            {
                response = $"Host={uriBuilder.Host}&{response}";
            }

            if (response.Length > 0 && response.Substring(response.Length - 1) == "&")
            {
                response = response.Substring(0, response.Length - 1);
            }

            if (response.Length > 0)
            {
                response = $"&{response}";
            }

            return response;
        }
    }
}