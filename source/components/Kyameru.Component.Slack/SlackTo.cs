﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kyameru.Component.Slack
{
    public class SlackTo : IToComponent
    {
        public event EventHandler<Log> OnLog;

        private readonly string[] allowedHeaders = new string[] { "Target" };

        private const string SLACKURI = "https://hooks.slack.com/services";
        private readonly Encoding _encoding = new UTF8Encoding();

        public Dictionary<string, string> headers;

        public SlackTo(Dictionary<string, string> incomingHeaders)
        {
            this.headers = this.ParseHeaders(incomingHeaders);
        }

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

        public void Process(Routable item)
        {
            payload slackPayload = new payload()
            {
                text = item.Headers["SlackMessage"]
            };
            var payloadJson = JsonSerializer.Serialize(slackPayload);
            string uri = $"{SLACKURI}{this.headers["Target"]}";
            var dataContent = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsync(uri, dataContent).Result;
                if (!response.IsSuccessStatusCode)
                {
                    // do something...probably log
                }
            }
        }
    }
}