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
        private AmazonSQSClient sqsClient;

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
            sqsClient = new AmazonSQSClient(awsConfig.AccessKey, awsConfig.SecretKey, awsConfig.Region);

        }
    }
}

