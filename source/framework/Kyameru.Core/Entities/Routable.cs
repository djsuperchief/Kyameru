using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    public class Routable
    {
        public Dictionary<string, string> Headers { get; private set; }

        public object Body { get; private set; }

        public Error Error { get; private set; }

        internal bool InError => this.Error != null;

        public Routable(Dictionary<string, string> headers, object data)
        {
            this.Headers = headers;
            this.Body = data;
        }

        internal void AddHeader(string key, string value)
        {
            if (!this.Headers.ContainsKey(key))
            {
                this.Headers.Add(key, value);
            }
            else
            {
                throw new Exceptions.ComponentException(Resources.ERROR_HEADER_IMMUTABLE);
            }
        }

        public void SetBody<T>(T value) where T : class
        {
            this.Body = value;
        }

        public void SetInError(Error error)
        {
            if (this.Error == null)
            {
                this.Error = error;
            }
        }
    }
}