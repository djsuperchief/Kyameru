using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
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
        /// <param name="identity">Identity of route.</param>
        public To(ILogger logger, IToComponent toComponent, string identity) : base(logger, identity)
        {
            this.toComponent = toComponent;
            this.toComponent.OnLog += this.OnLog;
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
                    this.Logger.KyameruException(this.identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("To Component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }
    }
}