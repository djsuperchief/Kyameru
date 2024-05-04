using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// Post processing component.
        /// </summary>
        private readonly IProcessComponent processComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="To"/> class.
        /// </summary>
        /// <param name="logger">Logger class.</param>
        /// <param name="toComponent">To component.</param>
        /// <param name="identity">Identity of route.</param>
        public To(ILogger logger, IToComponent toComponent, string identity) : base(logger, identity)
        {
            this.toComponent = toComponent;
            this.toComponent.OnLog += OnLog;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="To"/> class.
        /// </summary>
        /// <param name="logger">Logger class.</param>
        /// <param name="toComponent">To component.</param>
        /// <param name="postProcessComponent">Post processing component.</param>
        /// <param name="identity">Identity of route.</param>
        public To(ILogger logger, IToComponent toComponent, IProcessComponent postProcessComponent, string identity) :
            this(logger, toComponent, identity)
        {
            processComponent = postProcessComponent;
            this.processComponent.OnLog += OnLog;
        }

        /// <summary>
        /// Passes processing onto the next component.
        /// </summary>
        /// <param name="item">Message to process.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public override async Task HandleAsync(Routable item, CancellationToken cancellationToken)
        {
            if (!item.InError)
            {
                try
                {
                    await toComponent.ProcessAsync(item, cancellationToken);
                    if (processComponent != null)
                    {
                        await processComponent.ProcessAsync(item, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Logger.KyameruException(identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("To Component", "Handle", ex.Message));
                }
            }

            await base.HandleAsync(item, cancellationToken);
        }
    }
}