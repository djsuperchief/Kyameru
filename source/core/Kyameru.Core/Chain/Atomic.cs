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
        private readonly IAtomicComponent atomicComponent;

        public Atomic(ILogger logger, IAtomicComponent atomicComponent, string identity) : base(logger, identity)
        {
            this.atomicComponent = atomicComponent;
            this.atomicComponent.OnLog += OnLog;
        }

        public override void Handle(Routable item)
        {
            if (!item.InError)
            {
                try
                {
                    this.atomicComponent.Process(item);
                }
                catch (Exception ex)
                {
                    this.Logger.KyameruException(this.identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Atomic Component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }

        public override async Task HandleAsync(Routable item, CancellationToken cancellationToken)
        {
            if (!item.InError)
            {
                try
                {
                    await this.atomicComponent.ProcessAsync(item, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.KyameruException(this.identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Atomic Component", "Handle", ex.Message));
                }
            }
            await base.HandleAsync(item, cancellationToken);
        }
    }
}