using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Base chain for processing
    /// </summary>
    internal abstract class BaseChain : Contracts.IChain<Entities.Routable>
    {
        /// <summary>
        /// Logger interface.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseChain"/> class.
        /// </summary>
        /// <param name="logger">Logging class.</param>
        protected BaseChain(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets the next component.
        /// </summary>
        private IChain<Entities.Routable> Next { get; set; }

        /// <summary>
        /// Pass the processing onto the next component.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public virtual void Handle(Routable item)
        {
            this.Next?.Handle(item);
        }

        /// <summary>
        /// Sets the next component in the chain.
        /// </summary>
        /// <param name="next">Next component.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/>/> interface.</returns>
        public IChain<Routable> SetNext(IChain<Routable> next)
        {
            this.Next = next;
            return this.Next;
        }
    }
}