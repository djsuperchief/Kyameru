using System.Collections.Generic;

namespace Kyameru.Component.File
{
    /// <summary>
    /// Configuration extensions
    /// </summary>
    internal static class ConfigSetup
    {
        /// <summary>
        /// Valid from headers.
        /// </summary>
        private static string[] fromHeaders = new string[] { "Target", "Notifications", "Filter", "SubDirectories", "InitialScan", "Ignore", "IgnoreStrings" };

        private static Dictionary<string, string> optionalFromHeaders = new Dictionary<string, string>()
        {
            { "Filter", "*.*" },
            { "SubDirectories", "false" },
            { "InitialScan", "false" },
            { "Ignore", "" },
            { "IgnoreStrings", "" }
        };

        /// <summary>
        /// Valid to headers
        /// </summary>
        private static string[] toHeaders = new string[] { "Target", "Action", "Overwrite" };

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

            SetOptionalFromHeaders(response);

            return response;
        }

        private static void SetOptionalFromHeaders(Dictionary<string, string> response)
        {
            foreach(string key in optionalFromHeaders.Keys)
            {
                if(!response.ContainsKey(key))
                {
                    response.Add(key, optionalFromHeaders[key]);
                }
            }
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

            if(!response.ContainsKey("Overwrite"))
            {
                response.Add("Overwrite", "false");
            }

            return response;
        }
    }
}