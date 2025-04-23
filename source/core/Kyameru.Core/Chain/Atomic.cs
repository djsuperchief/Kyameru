using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class Atomic : BaseChain
    {
        private readonly IAtomicLink atomicComponent;

        public Atomic(ILogger logger, IAtomicLink atomicComponent, string identity) : base(logger, identity)
        {
            this.atomicComponent = atomicComponent;
            this.atomicComponent.OnLog += OnLog;
        }

        public override async Task HandleAsync(Routable item, CancellationToken cancellationToken)
        {
            if (!item.InError)
            {
                try
                {
                    await atomicComponent.ProcessAsync(item, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.KyameruException(identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Atomic Component", "Handle", ex.Message));
                }
            }
            await base.HandleAsync(item, cancellationToken);
        }
    }
}