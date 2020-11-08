using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class To : BaseChain
    {
        private readonly IToComponent toComponent;

        public To(ILogger logger, IToComponent toComponent) : base(logger)
        {
            this.toComponent = toComponent;
        }

        public override void Handle(Routable item)
        {
            this.toComponent.Process(item);
        }
    }
}
