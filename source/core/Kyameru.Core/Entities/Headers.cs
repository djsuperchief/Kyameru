using Kyameru.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Headers collection.
    /// </summary>
    public class Headers : IEnumerable<KeyValuePair<string, string>>
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
            headerStorage = headers.GetMutableValues();
            headerStorage.AddRange(headers.GetImmutableValues());
            immutable = headers.Keys.Where(x => x.Substring(0, 1) == "&").Select(x => x[1..]).ToList();
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
                return headerStorage[key];
            }
        }

        /// <summary>
        /// Gets a value indicating if the key exists.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns>Returns a boolean indicating whether the key exists.</returns>
        public bool ContainsKey(string key) => headerStorage.ContainsKey(key);

        /// <summary>
        /// Gets the count of items in the header storage.
        /// </summary>
        public int Count => headerStorage.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        /// <summary>
        /// Sets a header.
        /// </summary>
        /// <param name="key">Header key (prepended by &amp; indicates immutable).</param>
        /// <param name="value">Header Value.</param>
        public void SetHeader(string key, string value)
        {
            bool isImmutable = IsImmutable(key, out key);

            if (headerStorage.ContainsKey(key) && immutable.Contains(key))
            {
                throw new Exceptions.CoreException(Resources.ERROR_HEADER_IMMUTABLE);
            }

            if (headerStorage.ContainsKey(key))
            {
                headerStorage[key] = value;
            }
            else
            {
                headerStorage.Add(key, value);
            }
        }

        public string TryGetValue(string key)
        {
            var response = string.Empty;
            headerStorage.TryGetValue(key, out response);
            return response;
        }

        public string TryGetValue(string key, string defaultValue)
        {
            var response = string.Empty;
            headerStorage.TryGetValue(key, out response);
            if (string.IsNullOrWhiteSpace(response))
            {
                response = defaultValue;
            }

            return response;
        }

        /// <summary>
        /// Gets the internal header storage as a dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionary()
        {
            // Need to make this a clone or jst make this an enumerable.
            return headerStorage;
        }

        private bool IsImmutable(string key, out string editedKey)
        {
            bool response = false;
            if (key.Substring(0, 1) == "&")
            {
                response = true;
                key = key[1..];
                if (immutable.Count(x => x == key) > 0)
                {
                    new Exceptions.RouteDataException(string.Format(Resources.ERROR_HEADER_IMMUTABLE_ADDED, key));
                }
                immutable.Add(key);
            }

            editedKey = key;
            return response;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return headerStorage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}