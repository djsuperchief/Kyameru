using System;
using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    public class Routable
    {
        public Dictionary<string, string> Headers { get; private set; }

        public object Data { get; private set; }

        public Routable(Dictionary<string, string> headers, object data)
        {
            this.Headers = headers;
            this.Data = data;
        }

        internal void AddHeader(string key, string value)
        {
            this.Headers.Add(key, value);
        }
    }
}
