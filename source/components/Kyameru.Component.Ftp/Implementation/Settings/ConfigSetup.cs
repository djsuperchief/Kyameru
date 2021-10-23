using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Ftp.Settings
{
    /// <summary>
    /// Configuration extensions.
    /// </summary>
    public static class ConfigSetup
    {
        /// <summary>
        /// Valid from headers
        /// </summary>
        private static string[] fromHeaders = new string[] { "Target", "Host", "UserName", "Password", "Recursive", "PollTime", "Delete", "Port", "Filter" };

        private static string[] toHeaders = new string[] { "Target", "Host", "UserName", "Password", "Archive", "Source" };

        /// <summary>
        /// Converts incoming headers to valid processing headers.
        /// </summary>
        /// <param name="incoming">Incoming dictionary.</param>
        /// <returns>Returns a dictionary of valid headers.</returns>
        public static Dictionary<string, string> ToFromConfig(this Dictionary<string, string> incoming)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < fromHeaders.Length; i++)
            {
                if (incoming.ContainsKey(fromHeaders[i]))
                {
                    response.Add(fromHeaders[i], incoming[fromHeaders[i]]);
                }
            }

            CheckDefaultHeaders(response);

            return response;
        }

        /// <summary>
        /// Converts incoming headers to valid processing headers.
        /// </summary>
        /// <param name="incoming">Incoming dictionary.</param>
        /// <returns>Returns a dictionary of valid headers.</returns>
        public static Dictionary<string, string> ToToConfig(this Dictionary<string, string> incoming)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < toHeaders.Length; i++)
            {
                if (incoming.ContainsKey(toHeaders[i]))
                {
                    response.Add(toHeaders[i], incoming[toHeaders[i]]);
                }
            }

            CheckDefaultHeaders(response);

            return response;
        }

        /// <summary>
        /// Ensures required headers are populated.
        /// </summary>
        /// <param name="response"></param>
        private static void CheckDefaultHeaders(Dictionary<string, string> response)
        {
            response.SetDefault("PollTime", "60000");
            response.SetDefault("Filter", "*.*");
            response.SetDefault("Delete", "false");
            response.SetDefault("Recursive", "false");
            response.SetDefault("Port", "21");
        }

        /// <summary>
        /// Gets a key value from a dictionary.
        /// </summary>
        /// <param name="incoming">Incoming dictionary.</param>
        /// <param name="key">Key to find.</param>
        /// <returns>Returns an empty string if key not found.</returns>
        public static string GetKeyValue(this Dictionary<string, string> incoming, string key)
        {
            string response = string.Empty;
            if (incoming.ContainsKey(key))
            {
                response = incoming[key];
            }

            return response;
        }

        /// <summary>
        /// Sets a default value if it doesn't exist.
        /// </summary>
        /// <param name="incoming">Incoming dictionary.</param>
        /// <param name="key">Target key.</param>
        /// <param name="value">Required value.</param>
        public static void SetDefault(this Dictionary<string, string> incoming, string key, string value)
        {
            if (!incoming.ContainsKey(key))
            {
                incoming.Add(key, value);
            }
        }
    }
}