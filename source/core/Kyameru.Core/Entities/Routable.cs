using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Core Kyameru Message.
    /// </summary>
    public class Routable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Routable"/> class.
        /// </summary>
        /// <param name="headers">Headers to add.</param>
        /// <param name="data">Data to add.</param>
        public Routable(Dictionary<string, string> headers, object data)
        {
            this.Headers = new Headers(headers);
            this.Body = data;
        }

        /// <summary>
        /// Gets the processing headers.
        /// </summary>
        public Headers Headers { get; private set; }

        /// <summary>
        /// Gets the body of the message.
        /// </summary>
        public object Body { get; private set; }

        /// <summary>
        /// Gets the error of the message.
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the message is in error.
        /// </summary>
        internal bool InError => this.Error != null;

        /// <summary>
        /// Sets the body of the message.
        /// </summary>
        /// <typeparam name="T">Type for the body message.</typeparam>
        /// <param name="value">Value to set the body.</param>
        public void SetBody<T>(T value) where T : class
        {
            this.Body = value;
        }

        /// <summary>
        /// Sets the message to be in error.
        /// </summary>
        /// <param name="error">Error object.</param>
        public void SetInError(Error error)
        {
            if (this.Error == null)
            {
                this.Error = error;
            }
            else
            {
                error.InnerError = this.Error;
                this.Error = error;
            }
        }

        /// <summary>
        /// Adds or Sets a header to the message.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="value">Header value.</param>
        internal void SetHeader(string key, string value)
        {
            this.Headers.SetHeader(key, value);
        }
    }
}