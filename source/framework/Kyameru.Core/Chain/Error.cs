using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.Chain
{
    internal class Error : BaseChain
    {
        private readonly IErrorComponent errorComponent;

        public Error(ILogger logger, IErrorComponent errorComponent) : base(logger)
        {
            this.errorComponent = errorComponent;
            this.errorComponent.OnLog += this.ToComponent_OnLog;
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
            if (item.InError)
            {
                this.errorComponent.Process(item);
            }
        }
    }
}