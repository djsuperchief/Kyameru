using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Kyameru.Component.SQS.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS
{
    public class SqsClient
    {

        private readonly IAmazonSQS sqsClient;

        public SqsClient(IAmazonSQS client)
        {
            sqsClient = client;
        }


        /// <summary>
        /// Gets an AWS SQS Client based on the aws config.
        /// </summary>
        /// <param name="config">AWS config</param>
        /// <returns>Returns an instance of <see cref="IAmazonSQS"/></returns>
        public static IAmazonSQS GetSqsClient(AwsConfig config)
        {
            if (config.UseProfile)
            {
                return new AmazonSQSClient(); // Default will use credentials in profile etc / appsettings.
            }

            AWSCredentials credentials;
            if (!string.IsNullOrWhiteSpace(config.SessionToken))
            {
                credentials = new SessionAWSCredentials(config.AccessKey, config.SecretKey, config.SessionToken);
            }
            else
            {
                credentials = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            }

            var sqsConfig = new AmazonSQSConfig()
            {
                RegionEndpoint = config.Region,
                UseHttp = config.UseHttp
            };

            // This needs to be more elegant
            // Also this is done out of the initialisation block as it doesn't seem to work....for whatever reason.
            if (!string.IsNullOrWhiteSpace(config.ServiceUrl))
            {
                sqsConfig.ServiceURL = config.ServiceUrl;
            }

            if (!string.IsNullOrWhiteSpace(config.AuthenticationRegion))
            {
                sqsConfig.AuthenticationRegion = config.AuthenticationRegion;
            }

            return new AmazonSQSClient(credentials, sqsConfig);
        }

        public async Task SendMessage(AwsConfig config, Routable message)
        {
            var sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = config.Queue;
            sendMessageRequest.MessageBody = message.Body.ToString();
            sendMessageRequest.MessageAttributes = new Dictionary<string, MessageAttributeValue>();

        }

    }
}
