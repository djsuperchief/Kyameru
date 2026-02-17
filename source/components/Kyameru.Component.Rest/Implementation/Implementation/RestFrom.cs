using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Comms;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestFrom : CommonBase, IRestFrom
    {
        public RestFrom(IHttpContentFactory contentFactory, HttpMessageHandler? httpMessageHandler = null) : base(contentFactory, httpMessageHandler)
        {
        }
        
        public event AsyncEventHandler<RoutableEventData>? OnActionAsync;
        public void Setup()
        {
            // no setup required.
        }

        public async Task ProcessAsync(CommsMessage commsMessage, CancellationToken cancellationToken)
        {
            var httpMessageData = commsMessage.Data as Messages.HttpMessageData;
            if (httpMessageData == null)
            {
                Log(LogLevel.Error, Resources.ERROR_EVENTDATA_EMPTY);
            }
            
            if (httpMessageData!.Headers == null)
            {
                httpMessageData.Headers = new Dictionary<string, string>()
                {
                    { "KyameruMessageId", commsMessage.Id.ToString() }
                };
            }
            var routable = new Routable(httpMessageData.Headers, httpMessageData.Data);
            
            await SendAsync(routable, cancellationToken);
            OnActionAsync?.Invoke(this, new RoutableEventData(routable, cancellationToken));
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;
            ValidateHeaders();
        }
    }
}