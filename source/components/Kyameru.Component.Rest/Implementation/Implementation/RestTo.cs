using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {

        public event EventHandler<Log>? OnLog;
        public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            Headers = headers;
            ValidateHeaders();
        }
    }
}