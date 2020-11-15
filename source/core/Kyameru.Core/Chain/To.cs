using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// To component processing class.
    /// </summary>
    internal class To : BaseChain
    {
        /// <summary>
        /// To Component.
        /// </summary>
        private readonly IToComponent toComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="To"/> class.
        /// </summary>
        /// <param name="logger">Logger class.</param>
        /// <param name="toComponent">To component.</param>
        public To(ILogger logger, IToComponent toComponent) : base(logger)
        {
            this.toComponent = toComponent;
            this.toComponent.OnLog += this.ToComponent_OnLog;
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
                    this.toComponent.Process(item);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, ex.Message);
                    item.SetInError(new Entities.Error("To Component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        private void ToComponent_OnLog(object sender, Log e)
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