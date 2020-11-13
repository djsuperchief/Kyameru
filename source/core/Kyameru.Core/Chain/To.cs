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
            this.toComponent.OnLog += this.ToComponent_OnLog;
        }

        private void ToComponent_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                this.logger.Log(e.LogLevel, e.Message);
            }
            else
            {
                this.logger.LogError(e.Error, e.Message);
            }
        }

        public override void Handle(Routable item)
        {
            if (!item.InError)
            {
                try
                {
                    this.toComponent.Process(item);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, ex.Message);
                    item.SetInError(new Entities.Error("To Component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }
    }
}