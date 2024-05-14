using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Base chain for processing
    /// </summary>
    internal abstract class BaseChain : IChain<Routable>
    {
        /// <summary>
        /// Logger interface.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Route identity.
        /// </summary>
        protected readonly string identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseChain"/> class.
        /// </summary>
        /// <param name="logger">Logging class.</param>
        /// <param name="identity">Identity of route.</param>
        protected BaseChain(ILogger logger, string identity)
        {
            Logger = logger;
            this.identity = identity;
        }

        /// <summary>
        /// Gets or sets the next component.
        /// </summary>
        private IChain<Routable> Next { get; set; }

        /// <summary>
        /// Pass the processing onto the next component.
        /// </summary>
        /// <param name="item">Message to process.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public virtual async Task HandleAsync(Routable item, CancellationToken cancellationToken)
        {
            if (!item.ExitRoute)
            {
                // Really?
                if (Next != null)
                {
                    await Next?.HandleAsync(item, cancellationToken);
                }

            }
            else
            {
                Logger.KyameruWarning(identity, string.Format(Resources.WARNING_ROUTE_EXIT, item.ExitReason));
            }

        }

        /// <summary>
        /// Sets the next component in the chain.
        /// </summary>
        /// <param name="next">Next component.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/>/> interface.</returns>
        public IChain<Routable> SetNext(IChain<Routable> next)
        {
            Next = next;
            return Next;
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        protected void OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                Logger.KyameruLog(identity, e.Message, e.LogLevel);
            }
            else
            {
                Logger.KyameruError(identity, e.Message);
            }
        }
    }
}