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
            this.component.OnLog += this.Component_OnLog;
        }

        private void Component_OnLog(object sender, Log e)
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
                this.component.Process(item);
            }

            base.Handle(item);
        }
    }
}