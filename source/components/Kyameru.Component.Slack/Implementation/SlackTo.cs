using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Slack
{
    /// <summary>
    /// Main To Component.
    /// </summary>
    public class SlackTo : ISlackTo
    {
        /// <summary>
        /// Allowed headers.
        /// </summary>
        private readonly string[] allowedHeaders = new string[] { "Target", "MessageSource", "Username", "Channel" };

        /// <summary>
        /// Slack webhook URI
        /// </summary>
        private const string SLACKURI = "https://hooks.slack.com/services";

        /// <summary>
        /// Headers added.
        /// </summary>
        private readonly Dictionary<string, string> headers;

        /// <summary>
        /// Http message handler for test mocking.
        /// </summary>
        private readonly HttpMessageHandler httpHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlackTo"/> class.
        /// </summary>
        /// <param name="incomingHeaders">Incoming Headers.</param>
        /// <param name="handler">Http Message handler for unit test mocking.</param>
        public SlackTo(Dictionary<string, string> incomingHeaders, HttpMessageHandler handler = null)
        {
            this.headers = this.ParseHeaders(incomingHeaders);
            this.httpHandler = handler;
        }

        /// <summary>
        /// Event raised for logging.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Parses headers that are valid.
        /// </summary>
        /// <param name="incomingHeaders">Incoming headers.</param>
        /// <returns>Returns a dictionary of valid headers.</returns>
        private Dictionary<string, string> ParseHeaders(Dictionary<string, string> incomingHeaders)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            for (int i = 0; i < this.allowedHeaders.Length; i++)
            {
                if (incomingHeaders.ContainsKey(this.allowedHeaders[i]))
                {
                    response.Add(this.allowedHeaders[i], incomingHeaders[this.allowedHeaders[i]]);
                }
            }

            return response;
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            Payload slackPayload = new Payload()
            {
                text = this.GetMessageSource(routable),
                channel = this.GetHeader("Channel"),
                username = this.GetHeader("Username")
            };

            if (routable.Error == null)
            {
                var payloadJson = JsonSerializer.Serialize(slackPayload);
                string uri = $"{SLACKURI}{this.headers["Target"]}";
                var dataContent = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                using (HttpClient client = this.GetHttpClient())
                {
                    OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Sending slack message"));
                    var response = await client.PostAsync(uri, dataContent, cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        routable.SetInError(this.RaiseError("SendSlackMessage", "Error communicating with slack."));
                    }
                }
            }
        }

        /// <summary>
        /// Raises an error.
        /// </summary>
        /// <param name="action">Current action.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Returns an instance of the <see cref="Error"/> class.</returns>
        private Error RaiseError(string action, string message)
        {
            return new Error("Slack", action, message);
        }

        /// <summary>
        /// Gets a HttpClient.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="HttpClient"/> class.</returns>
        private HttpClient GetHttpClient()
        {
            HttpClient response;
            if (this.httpHandler == null)
            {
                response = new HttpClient();
            }
            else
            {
                response = new HttpClient(this.httpHandler);
            }

            return response;
        }

        /// <summary>
        /// Gets the message source.
        /// </summary>
        /// <param name="routable">Message to process.</param>
        /// <returns>Returns the message to send via slack.</returns>
        private string GetMessageSource(Routable routable)
        {
            string response = string.Empty;
            if (this.headers.ContainsKey("MessageSource") && this.headers["MessageSource"].ToLower() == "body")
            {
                response = (string)routable.Body;
            }
            else if (routable.Headers.ContainsKey("SlackMessage"))
            {
                response = routable.Headers["SlackMessage"];
            }
            else
            {
                routable.SetInError(this.RaiseError("GettingMessageSource", "Error getting message source."));
            }



            return response;
        }

        private string GetHeader(string header)
        {
            string response = string.Empty;
            if (this.headers.ContainsKey(header))
            {
                response = this.headers[header];
            }

            return response;
        }
    }
}