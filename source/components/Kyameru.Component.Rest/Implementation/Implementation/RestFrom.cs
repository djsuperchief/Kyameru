using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestFrom : CommonBase, IRestFrom
    {
        public event EventHandler<Log> OnLog;
        public event AsyncEventHandler<RoutableEventData> OnActionAsync;

        public RestFrom(IKeyedServiceProvider keyedServiceProvider,HttpMessageHandler httpMessageHandler = null) : base(keyedServiceProvider,httpMessageHandler)
        {
        }

        public void Setup()
        {
            // do nothing.
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var authenticationStrategy = KeyedServiceProvider.GetRequiredService<IAuthenticationStrategy>();
            using (var client = this.GetHttpClient())
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = new HttpMethod(_headers["method"]),
                    RequestUri = new Uri(Url)
                };
                await authenticationStrategy.ApplyAsyc(client);
                var response = await client.SendAsync(httpRequest, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var routable = new Routable(new Dictionary<string, string>(), response.Content);
                    foreach (var header in response.Headers)
                    {
                        routable.SetHeader(header.Key, header.Value.ToString());
                    }

                    OnActionAsync?.Invoke(this, new RoutableEventData(routable, cancellationToken));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // nothing to do...yet
            return Task.CompletedTask;
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            ValidateHeaders();
        }
    }
}