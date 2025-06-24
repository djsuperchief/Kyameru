using System;
using System.Collections.Generic;
using System.Linq;
using Kyameru.Component.Rest.Extensions;

namespace Kyameru.Component.Rest.Implementation
{
    public abstract class CommonBase
    {
        protected Dictionary<string, string> _headers;
        protected readonly string[] _requiredHeaders = new string[]
        {
            "endpoint",
            "Host",
            "Target"
        };

        protected readonly string[] _validMethods = new string[]
        {
            "post",
            "get",
            "put",
            "delete"
        };


        public string HttpMethod { get; protected set; }

        public string Url { get; protected set; }

        protected void SetUrl()
        {
            var toRemove = _requiredHeaders.Union(new string[] { "method" }).ToArray();
            Url = _headers.ToValidApiEndpoint(toRemove);
        }

        protected void ValidateHeaders()
        {
            foreach (var required in _requiredHeaders)
            {
                if (!_headers.ContainsKey(required) || string.IsNullOrWhiteSpace(_headers[required]))
                {
                    throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_MISSINGHEADER, required));
                }
            }

            if (!_headers.ContainsKey("method"))
            {
                _headers["method"] = "get";
            }

            if (!_validMethods.Any(x => x == _headers["method"]))
            {
                throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_INVALID_METHOD, _headers["method"]));
            }

            HttpMethod = _headers["method"];
            SetUrl();
        }
    }
}