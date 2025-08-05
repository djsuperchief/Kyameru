using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kyameru.Component.Rest.Extensions;

namespace Kyameru.Component.Rest.Implementation
{
    public abstract class CommonBase
    {
        protected Dictionary<string, string> _headers;

        protected readonly HttpMessageHandler httpHandler;
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
            "delete",
            "connect",
            "head",
            "options",
            "trace",
            "patch"
        };

        protected CommonBase(HttpMessageHandler httpMessageHandler = null)
        {
            httpHandler = httpMessageHandler;
        }

        public HttpMethod HttpMethod { get; private set; }

        public string Url { get; private set; }

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

            if (_validMethods.All(x => x != _headers["method"].ToLower()))
            {
                throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_INVALID_METHOD, _headers["method"]));
            }

            HttpMethod = new HttpMethod(_headers["method"]);
            SetUrl();
        }

        protected HttpClient GetHttpClient()
        {
            HttpClient response;
            if (httpHandler == null)
            {
                response = new HttpClient();
            }
            else
            {
                response = new HttpClient(httpHandler);
            }

            return response;
        }
    }
}