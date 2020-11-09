using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kyameru.Component.File
{
    internal static class ConfigSetup
    {
        private static string[] headers = new string[] { "Target", "Notifications", "Filter", "SubDirectories" };

        public static Dictionary<string, string> ToConfig(this Dictionary<string, string> incoming)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length; i++)
            {
                if (incoming.ContainsKey(headers[i]))
                {
                    response.Add(headers[i], incoming[headers[i]]);
                }
            }

            if (!response.ContainsKey("Filter"))
            {
                response.Add("Filter", "*.*");
            }

            return response;
        }

        public static Dictionary<string, string> ToConfig(this string[] args)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > headers.Length)
                {
                    break;
                }

                response.Add(headers[i], args[i]);
            }

            if (!response.ContainsKey("Filter"))
            {
                response.Add("Filter", "*.*");
            }

            return response;
        }
    }
}