using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyameru.Core.Entities
{
    public class Headers
    {
        /// <summary>
        /// Header dictionary.
        /// </summary>
        private Dictionary<string, string> headerStorage = new Dictionary<string, string>();

        /// <summary>
        /// List of immutable header keys.
        /// </summary>
        private List<string> immutable = new List<string>();

        public Headers(Dictionary<string, string> headers)
        {
            this.headerStorage = headers;
            this.immutable = this.headerStorage.Keys.ToList();
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


            if(this.headerStorage.ContainsKey(key))
            {
                this.headerStorage[key] = value;
            }
            else
            {
                this.headerStorage.Add(key, value);
            }

            if(isImmutable)
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