using System;
using System.Threading;
using Kyameru.Core.Entities;

namespace Kyameru.Core
{
    public class RoutableEventData
    {
        public CancellationToken CancellationToken { get; private set; }

        public Routable Data { get; private set; }

        public RoutableEventData(Routable data, CancellationToken cancellationToken)
        {
            Data = data;
            CancellationToken = cancellationToken;
        }
    }
}
