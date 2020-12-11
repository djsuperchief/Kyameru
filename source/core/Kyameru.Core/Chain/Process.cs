using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
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
        /// <param name="identity">Identity of route.</param>
        public Process(ILogger logger, IProcessComponent processComponent, string identity) : base(logger, identity)
        {
            this.component = processComponent;
            this.component.OnLog += this.OnLog;
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
                    this.Logger.KyameruException(this.identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Processing component", "Handle", ex.Message));
                }
            }

            base.Handle(item);
        }
    }
}