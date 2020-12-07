using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class Atomic : BaseChain
    {
        private readonly IAtomicComponent atomicComponent;

        

        public Atomic(ILogger logger, IAtomicComponent atomicComponent) : base(logger)
        {
            this.atomicComponent = atomicComponent;
            this.atomicComponent.OnLog += AtomicComponent_OnLog;
        }

        public override void Handle(Routable item)
        {
            if(!item.InError)
            {
                try
                {
                    this.atomicComponent.Process(item);
                }
                catch(Exception ex)
                {
                    this.Logger.LogError(ex, ex.Message);
                    item.SetInError(new Entities.Error("Atomic Component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }

        private void AtomicComponent_OnLog(object sender, Entities.Log e)
        {
            if (e.Error == null)
            {
                this.Logger.Log(e.LogLevel, e.Message);
            }
            else
            {
                this.Logger.LogError(e.Error, e.Message);
            }
        }
    }
}
