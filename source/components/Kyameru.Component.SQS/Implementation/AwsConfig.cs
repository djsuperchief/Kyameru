using System;
using System.Collections.Generic;
using Amazon.Runtime.Internal.Transform;

namespace Kyameru.Component.SQS
{
    public class AwsConfig
    {
        public string ServiceUrl { get; private set; }

        public string AccessKey { get; private set; }

        public string SecretKey { get; private set; }

        public Dictionary<string, string> OtherHeaders { get; set; }

        private readonly Dictionary<string, Action<string>> PropertyAssignment = new Dictionary<string, Action<string>>();

        public AwsConfig(
            string serviceUrl,
            string accessKey,
            string secretKey,
            Dictionary<string, string>? otherHeaders)
        {
            ServiceUrl = serviceUrl;
            AccessKey = accessKey;
            SecretKey = secretKey;
            OtherHeaders = otherHeaders ?? new Dictionary<string, string>();
        }

        public AwsConfig(Dictionary<string, string> headers) : this()
        {
            foreach (var key in headers.Keys)
            {
                if (PropertyAssignment.ContainsKey(key))
                {
                    PropertyAssignment[key](headers[key]);
                }
                else
                {
                    AddToOther(key, headers[key]);
                }
            }
        }

        private AwsConfig()
        {
            PropertyAssignment.Add("serviceurl", SetServiceUrl);
            PropertyAssignment.Add("accesskey", SetAccessKey);
            PropertyAssignment.Add("secretkey", SetSecretKey);
        }

        public void SetServiceUrl(string value) => ServiceUrl = value;

        private void SetAccessKey(string value) => AccessKey = value;

        private void SetSecretKey(string value) => SecretKey = value;

        private void AddToOther(string key, string value) => OtherHeaders.Add(key, value);
    }
}

