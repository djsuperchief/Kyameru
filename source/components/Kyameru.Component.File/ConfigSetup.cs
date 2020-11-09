using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kyameru.Component.File
{
    internal static class ConfigSetup
    {
        private static string[] fromHeaders = new string[] { "Target", "Notifications", "Filter", "SubDirectories" };
        private static string[] toHeaders = new string[] { "Target", "Action" };

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

            if (!response.ContainsKey("Filter"))
            {
                response.Add("Filter", "*.*");
            }

            return response;
        }

        public static Dictionary<string, string> ToFromConfig(this string[] args)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > fromHeaders.Length)
                {
                    break;
                }

                response.Add(fromHeaders[i], args[i]);
            }

            if (!response.ContainsKey("Filter"))
            {
                response.Add("Filter", "*.*");
            }

            return response;
        }

        public static Dictionary<string, string> ToToConfig(this string[] args)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > toHeaders.Length)
                {
                    break;
                }

                response.Add(toHeaders[i], args[i]);
            }

            return response;
        }

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

            return response;
        }
    }
}