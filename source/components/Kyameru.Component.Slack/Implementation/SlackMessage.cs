using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Kyameru.Component.Slack
{
    internal class SlackMessage
    {
        public Payload Payload { get; internal set; }
        public string Uri { get; internal set; }

        public StringContent DataContent
        {
            get
            {
                var payloadJson = JsonSerializer.Serialize(Payload);
                return new StringContent(payloadJson, Encoding.UTF8, "application/json");
            }
        }
    }
}
