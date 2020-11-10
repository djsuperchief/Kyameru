using System;
using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    public class Routable
    {
        public Dictionary<string, string> Headers { get; private set; }

        public object Body { get; private set; }

        internal bool InError => this.Headers.ContainsKey("InError");

        public Routable(Dictionary<string, string> headers, object data)
        {
            this.Headers = headers;
            this.Body = data;
        }

        internal void AddHeader(string key, string value)
        {
            this.Headers.Add(key, value);
        }

        public void SetBody<T>(T value) where T : class
        {
            this.Body = value;
        }

        public void SetInError(string source)
        {
            if (!this.Headers.ContainsKey("InError"))
            {
                this.Headers.Add("InError", source);
            }
        }
    }
}