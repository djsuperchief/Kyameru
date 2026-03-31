using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Extensions;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Rest.Implementation
{
    public abstract class CommonBase
    {
        private readonly IHttpContentFactory _httpContentFactory;
        private readonly List<string> _acceptedBodyRequests = new List<string>()
        {
            "PUT", "POST", "PATCH"
        };
        
        public event EventHandler<Log>? OnLog;
        
        protected Dictionary<string, string> Headers =  new Dictionary<string, string>();
        
        protected readonly IServiceProvider _keyedServiceProvider;
        
        private readonly HttpMessageHandler? _httpHandler;

        public HttpMethod HttpMethod { get; private set; } = null!;

        public string Url { get; private set; } = null!;
        
        public void AddAuthDependencyId(Guid id)
        {
            AuthDependencyId = id;
        }
        
        protected CommonBase(IHttpContentFactory contentFactory, IServiceProvider keyedServiceProvider, HttpMessageHandler? httpMessageHandler = null)
        {
            _httpHandler = httpMessageHandler;
            _httpContentFactory = contentFactory;
            _keyedServiceProvider = keyedServiceProvider;
        }
        
        private readonly string[] _requiredHeaders = new string[]
        {
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

        private Guid AuthDependencyId;


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

        private HttpClient GetHttpClient()
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
            OnLog?.Invoke(this, new Log(logLevel, message, exception));
        }

        protected async Task SendAsync(Routable routable, CancellationToken cancellationToken)
        {
            var authStrategy = _keyedServiceProvider.GetRequiredKeyedService<IAuthStrategy>(AuthDependencyId);
            if (_acceptedBodyRequests.Contains(Headers["method"]) && routable.Body != null)
            {
                await SendWithBody(routable, authStrategy, cancellationToken);
            }
            else
            {
                await SendWithoutBody(routable, authStrategy, cancellationToken);
            }
        }

        private async Task SendWithoutBody(Routable routable, IAuthStrategy authStrategy,
            CancellationToken cancellationToken)
        {
            using var client = GetHttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod(Headers["method"]),
                RequestUri = new Uri(Url)
            };
            await authStrategy.ApplyAsync(client);
            var response = await client.SendAsync(httpRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                routable.SetBody(response.Content);
            }
            else
            {
                Log(LogLevel.Error, string.Format(Resources.ERROR_REQUEST,  response.ReasonPhrase));
                routable.SetInError(new Error("Rest", "Sending Without Body", string.Format(Resources.ERROR_REQUEST,  response.ReasonPhrase)));
            }
        }

        private async Task SendWithBody(Routable routable, IAuthStrategy authStrategy,
            CancellationToken cancellationToken)
        {
            using var client = GetHttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod(Headers["method"]),
                RequestUri = new Uri(Url),
                Content = _httpContentFactory.Create(routable)
            };
            await authStrategy.ApplyAsync(client);
            var response = await client.SendAsync(httpRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                routable.SetBody(response.Content);
            }
            else
            {
                Log(LogLevel.Error, string.Format(Resources.ERROR_REQUEST,  response.ReasonPhrase));
                routable.SetInError(new Error("Rest", "Sending Without Body", string.Format(Resources.ERROR_REQUEST,  response.ReasonPhrase)));
            }
        }

        private void CheckEndpointHeader()
        {
            if ((!Headers.ContainsKey("endpoint") || string.IsNullOrWhiteSpace(Headers["endpoint"]))
                && (!string.IsNullOrWhiteSpace(Headers["Host"]) && !string.IsNullOrWhiteSpace(Headers["Port"])))
            {
                Headers.TryAdd("endpoint", $"{Headers["Host"]}:{Headers["Port"]}");
            }
        }
        
        private void SetUrl()
        {
            var toRemove = _requiredHeaders.Union(new string[] { "method", "Port" }).ToArray();
            Url = Headers.ToValidApiEndpoint(toRemove);
        }
    }
}