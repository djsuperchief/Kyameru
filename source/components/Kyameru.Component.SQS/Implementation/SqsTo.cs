using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS
{
    public class SqsTo : IToComponent
    {
        //private Dictionary<string, string> headers;

        private AwsConfig awsConfig;
        private IAmazonSQS sqsClient;

        public event EventHandler<Log> OnLog;

        public SqsTo(Dictionary<string, string> headers)
        {
            awsConfig = headers.ParseHeadersToAwsConfig();
        }

        public void Process(Routable item)
        {

        }

        private async Task SendSqsMessage(Routable item)
        {
            foreach (var header in awsConfig.OtherHeaders)
            {
                // add the extra headers to the message headers. Currently this includes everything.
                item.SetHeader(header.Key, header.Value);
            }
            var awsClient = SqsClient.GetSqsClient(awsConfig);
            var sqsClient = new SqsClient(awsClient);

        }
    }
}

