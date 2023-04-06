using System;
using System.Collections.Generic;

namespace Kyameru.Component.SQS
{
    public class SqsMessage
    {
        public SqsMessage() : this(string.Empty, new Dictionary<string, string>())
        {
        }

        public SqsMessage(string body) : this(body, new Dictionary<string, string>())
        {
        }

        public SqsMessage(string body, Dictionary<string, string> headers)
        {
            Headers = headers;
            Body = body;
        }

        public string Body { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }
    }
}

