using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Kyameru.Core.Exceptions;

namespace Kyameru.Component.SQS
{
    public class AwsConfig
    {
        public string Profile { get; private set; }

        public string ServiceUrl { get; private set; }

        public string AccessKey { get; private set; }

        public string SecretKey { get; private set; }

        public string SessionToken { get; private set; }

        public RegionEndpoint Region { get; private set; }

        public string Queue { get; private set; }

        public bool UseHttp { get; private set; }

        public Dictionary<string, string> OtherHeaders { get; private set; }

        private readonly Dictionary<string, Action<string>> PropertyAssignment = new Dictionary<string, Action<string>>();

        private readonly string[] _requiredHeaders = new string[] { "queue" };

        public AwsConfig(Dictionary<string, string> headers) : this()
        {
            foreach (var required in _requiredHeaders)
            {
                if (!headers.ContainsKey(required))
                {
                    throw new ActivationException(string.Format(Resources.ERROR_MISSING_HEADER, required), "SQS_Config");
                }
            }

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

            // region must be present but specify a default of eu-west-2
            if (!headers.ContainsKey("region"))
            {
                PropertyAssignment["region"]("eu-west-2");
            }
        }

        private AwsConfig()
        {
            PropertyAssignment.Add("serviceurl", SetServiceUrl);
            PropertyAssignment.Add("accesskey", SetAccessKey);
            PropertyAssignment.Add("secretkey", SetSecretKey);
            PropertyAssignment.Add("region", SetRegionKey);
            PropertyAssignment.Add("sessiontoken", SetSessionToken);
            PropertyAssignment.Add("profile", SetProfile);
            PropertyAssignment.Add("usehttp", SetUseHttp);
            PropertyAssignment.Add("queue", SetQueue);
            OtherHeaders = new Dictionary<string, string>();
        }

        public void SetServiceUrl(string value) => ServiceUrl = value;

        private void SetAccessKey(string value) => AccessKey = value;

        private void SetSecretKey(string value) => SecretKey = value;

        private void SetRegionKey(string value) => Region = RegionEndpoint.GetBySystemName(value);

        private void SetSessionToken(string value) => SessionToken = value;

        private void SetProfile(string value) => Profile = value;

        private void SetUseHttp(string value) => UseHttp = bool.Parse(value);

        private void SetQueue(string value) => Queue = value;

        private void AddToOther(string key, string value) => OtherHeaders.Add(key, value);
    }
}

