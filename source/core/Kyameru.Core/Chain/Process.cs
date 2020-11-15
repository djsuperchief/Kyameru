using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Intermediary processing component.
    /// </summary>
    internal class Process : BaseChain
    {
        /// <summary>
        /// Core processing component.
        /// </summary>
        private readonly IProcessComponent component;

        /// <summary>
        /// Initializes a new instance of the <see cref="Process"/> class.
        /// </summary>
        /// <param name="logger">Logger class.</param>
        /// <param name="processComponent">Processing component.</param>
        public Process(ILogger logger, IProcessComponent processComponent) : base(logger)
        {
            this.component = processComponent;
            this.component.OnLog += this.Component_OnLog;
        }

        /// <summary>
        /// Passes processing onto the next component.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public override void Handle(Routable item)
        {
            if (!item.InError)
            {
                try
                {
                    this.component.Process(item);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, ex.Message);
                    item.SetInError(new Entities.Error("Processing component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        private void Component_OnLog(object sender, Log e)
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