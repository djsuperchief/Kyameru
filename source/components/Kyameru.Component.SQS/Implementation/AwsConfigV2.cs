using System;
using System.Collections.Generic;
using Amazon;
using Amazon.SQS;
using Amazon.Runtime;
using System.Linq;

namespace Kyameru.Component.SQS
{
    // Will need to be renamed, not sure what i was thinking on the other one.
    public class AwsConfigV2
    {
        public string Profile { get; private set; }

        // This is so that if profile is mentioned (as in AWS profile) then none of the other properties of the client
        // are loaded in. Profile overrides all.
        private bool KeysSepcified;

        public string AccessKey { get; private set; }

        public string SecretKey { get; private set; }

        public string Region { get; private set; }

        public string SessionToken { get; private set; }

        // Not required but can be used for testing.
        public string EndpointUrl { get; private set; }

        public string queue { get; private set; }

        // Headers that must be present.
        private static string[] requiredHeaders = new string[] { "queue" };

        public AwsConfigV2()
        {

        }

        // public IAmazonSQS GetSqsClient()
        // {

        // }

        public static AwsConfigV2 Parse(Dictionary<string, string> headers)
        {
            foreach (var header in requiredHeaders)
            {
                if (!headers.ContainsKey(header))
                {
                    throw new Exceptions.MissingRequiredHeaderException(string.Format(Resources.ERROR_MISSING_HEADER, header));
                }
            }

            if (headers.ContainsKey("profile"))
            {
                // Only do the profile name
            }

            return new AwsConfigV2();
        }
    }
}
