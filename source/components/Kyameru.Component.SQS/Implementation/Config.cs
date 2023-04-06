using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;

namespace Kyameru.Component.SQS
{
    public static class Config
    {
        // These are required.
        private static string[] CommonRequiredHeaders = new string[] { "region", "queue" };

        private static Dictionary<string, string> CommonOptionalHeaders = new Dictionary<string, string>()
        {
            { "serviceurl", string.Empty },
            { "accesskey", string.Empty },
            { "secretkey", string.Empty }
        };

        public static Dictionary<string, string> ParseHeaders(this Dictionary<string, string> incoming)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < CommonRequiredHeaders.Length; i++)
            {
                if (incoming.ContainsKey(CommonRequiredHeaders[i]))
                {
                    response.Add(CommonRequiredHeaders[i], incoming[CommonRequiredHeaders[i]]);
                }
                else
                {
                    throw new ActivationException(string.Format(Resources.ERROR_MISSING_HEADER, CommonRequiredHeaders[i]), "From");
                }
            }

            SetOptionalHeaders(response);

            return response;
        }

        public static AwsConfig ParseHeadersToConfig(this Dictionary<string, string> incoming)
        {
            return new AwsConfig(incoming);
        }

        private static void SetOptionalHeaders(Dictionary<string, string> response)
        {
            foreach (string key in CommonOptionalHeaders.Keys)
            {
                if (!response.ContainsKey(key))
                {
                    response.Add(key, CommonOptionalHeaders[key]);
                }
            }
        }
    }
}

