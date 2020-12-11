using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Error processing.
    /// </summary>
    internal class Error : BaseChain
    {
        /// <summary>
        /// Error component.
        /// </summary>
        private readonly IErrorComponent errorComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="logger">Logging interface.</param>
        /// <param name="errorComponent">Error component.</param>
        /// <param name="identity">Route identity.</param>
        public Error(ILogger logger, IErrorComponent errorComponent, string identity) : base(logger, identity)
        {
            this.errorComponent = errorComponent;
            this.errorComponent.OnLog += this.OnLog;
        }

        /// <summary>
        /// Passes processing to the next in the chain.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public override void Handle(Routable item)
        {
            if (item.InError)
            {
                try
                {
                    this.errorComponent.Process(item);
                }
                catch (Exception ex)
                {
                    this.Logger.KyameruException(this.identity, ex.Message, ex);
                }
            }
        }
    }
}