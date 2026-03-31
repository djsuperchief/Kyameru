using System;
using System.Collections.Generic;
using Kyameru.Core.Comms;

namespace Kyameru.Component.Rest.Messages
{
    public class HttpMessageData
    {
        public Dictionary<string, string>? Headers { get; set; }
        public object Data { get; set; } = null!;

        public static HttpMessageData Create(object data, Dictionary<string, string>? headers = null)
        {
            return new HttpMessageData()
            {
                Headers = headers,
                Data = data
            };
        }
    }
}