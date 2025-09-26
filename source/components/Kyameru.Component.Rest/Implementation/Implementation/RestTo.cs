using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {
        

        public RestTo(HttpMessageHandler httpMessageHandler = null) : base(httpMessageHandler)
        {
        }
        
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            using (var client = GetHttpClient())
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = new HttpMethod(Headers["method"]),
                    RequestUri = new Uri(Url)
                };
                var response = await client.SendAsync(httpRequest, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    routable.SetBody(response.Content);
                }
                else
                {
                    Log(LogLevel.Error, string.Format(Resources.ERROR_REQUEST,  response.ReasonPhrase));
                }
            }
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;
            ValidateHeaders();
        }
    }
}