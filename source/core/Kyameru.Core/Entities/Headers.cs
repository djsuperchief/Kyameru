using Kyameru.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Headers collection.
    /// </summary>
    public class Headers
    {
        /// <summary>
        /// Header dictionary.
        /// </summary>
        private readonly Dictionary<string, string> headerStorage;

        /// <summary>
        /// List of immutable header keys.
        /// </summary>
        private readonly List<string> immutable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Headers"/> class.
        /// </summary>
        /// <param name="headers">Headers to create.</param>
        public Headers(Dictionary<string, string> headers)
        {
            this.headerStorage = headers.GetMutableValues();
            this.headerStorage.AddRange(headers.GetImmutableValues());
            this.immutable = headers.Keys.Where(x => x.Substring(0, 1) == "&").Select(x => x[1..]).ToList();
        }

        /// <summary>
        /// Gets a header value by key.
        /// </summary>
        /// <param name="key">Dictionary key</param>
        /// <returns>Dictionary value.</returns>
        public string this[string key]
        {
            get
            {
                return this.headerStorage[key];
            }
        }

        /// <summary>
        /// Gets a value indicating if the key exists.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>Returns a boolean indicating whether the key exists.</returns>
        public bool ContainsKey(string key) => this.headerStorage.ContainsKey(key);

        /// <summary>
        /// Sets a header.
        /// </summary>
        /// <param name="key">Header key (prepended by &amp; indicates immutable).</param>
        /// <param name="value">Header Value.</param>
        public void SetHeader(string key, string value)
        {
            bool isImmutable = this.IsImmutable(key, out key);

            if (this.headerStorage.ContainsKey(key) && this.immutable.Contains(key))
            {
                throw new Exceptions.CoreException(Resources.ERROR_HEADER_IMMUTABLE);
            }

            if (this.headerStorage.ContainsKey(key))
            {
                this.headerStorage[key] = value;
            }
            else
            {
                this.headerStorage.Add(key, value);
            }

            if (isImmutable)
            {
                this.immutable.Add(key);
            }
        }

        private bool IsImmutable(string key, out string editedKey)
        {
            bool response = false;
            if (key.Substring(0, 1) == "&")
            {
                response = true;
                key = key[1..];
                this.immutable.Add(key);
            }

            editedKey = key;
            return response;
        }
    }
}