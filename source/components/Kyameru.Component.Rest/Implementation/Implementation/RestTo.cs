using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {


        public RestTo(IHttpContentFactory contentFactory, IKeyedServiceProvider keyedServiceProvider,
            HttpMessageHandler? httpMessageHandler = null) : base(contentFactory, keyedServiceProvider,
            httpMessageHandler)
        {
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            await SendAsync(routable, cancellationToken);
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;
            ValidateHeaders();
        }
    }
}