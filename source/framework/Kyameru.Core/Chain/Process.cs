using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class Process : BaseChain
    {
        private readonly IProcessComponent component;

        public Process(ILogger logger, IProcessComponent processComponent) : base(logger)
        {
            this.component = processComponent;
        }

        public override void Handle(Routable item)
        {
            this.component.Process(item);
            base.Handle(item);
        }
    }
}
