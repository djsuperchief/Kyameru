using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {
        public event EventHandler<Log> OnLog;

        public RestTo(HttpMessageHandler httpMessageHandler = null) : base(httpMessageHandler)
        {
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            using (HttpClient client = this.GetHttpClient())
            {
                var response = await client.GetAsync(Url, cancellationToken);
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