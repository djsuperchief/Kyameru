using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {
        public event EventHandler<Log> OnLog;

        public RestTo(IKeyedServiceProvider keyedServiceProvider, HttpMessageHandler httpMessageHandler = null) : base(
            keyedServiceProvider, httpMessageHandler)
        {
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            using (HttpClient client = this.GetHttpClient())
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = new HttpMethod(_headers["method"]),
                    RequestUri = new Uri(Url)
                };
                var response = await client.SendAsync(httpRequest, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    routable.SetBody(response.Content);
                }
            }
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            ValidateHeaders();
        }
    }
}