using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal abstract class BaseChain : Contracts.IChain<Entities.Routable>
    {
        private readonly ILogger logger;
        private IChain<Entities.Routable> Next { get; set; }

        public BaseChain(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual void Handle(Routable item)
        {
            this.Next?.Handle(item);
        }

        public void Log(string logText)
        {
            throw new NotImplementedException();
        }

        public IChain<Routable> SetNext(IChain<Routable> next)
        {
            this.Next = next;
            return this.Next;
        }
    }
}
