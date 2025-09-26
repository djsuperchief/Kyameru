using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kyameru.Component.Rest.Extensions;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Rest.Implementation
{
    public abstract class CommonBase
    {
        public event EventHandler<Log>? OnLog;
        
        protected Dictionary<string, string> Headers =  new Dictionary<string, string>();
        
        private readonly HttpMessageHandler _httpHandler;

        public HttpMethod HttpMethod { get; private set; } = null!;

        public string Url { get; private set; } = null!;
        
        protected CommonBase(HttpMessageHandler httpMessageHandler = null)
        {
            _httpHandler = httpMessageHandler;
        }
        
        private readonly string[] _requiredHeaders = new string[]
        {
            "endpoint",
            "Host",
            "Target"
        };

        private readonly string[] _validMethods = new string[]
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
        
        protected void ValidateHeaders()
        {
            foreach (var required in _requiredHeaders)
            {
                if (!Headers.ContainsKey(required) || string.IsNullOrWhiteSpace(Headers[required]))
                {
                    throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_MISSINGHEADER, required));
                }
            }

            Headers.TryAdd("method", "get");

            if (_validMethods.All(x => x != Headers["method"].ToLower()))
            {
                throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_INVALID_METHOD, Headers["method"]));
            }

            HttpMethod = new HttpMethod(Headers["method"]);
            SetUrl();
        }
        
        private void SetUrl()
        {
            var toRemove = _requiredHeaders.Union(new string[] { "method" }).ToArray();
            Url = Headers.ToValidApiEndpoint(toRemove);
        }
        
        protected HttpClient GetHttpClient()
        {
            HttpClient response;
            if (_httpHandler == null)
            {
                response = new HttpClient();
            }
            else
            {
                response = new HttpClient(_httpHandler);
            }

            return response;
        }
        
        protected void Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}