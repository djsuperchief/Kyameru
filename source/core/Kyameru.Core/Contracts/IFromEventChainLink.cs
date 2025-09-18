using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Comms;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// From event chain link.
    /// </summary>
    public interface IFromEventChainLink : IComponent
    {
        /// <summary>
        /// Event raised to trigger processing the chain async.
        /// </summary>
        event AsyncEventHandler<RoutableEventData> OnActionAsync;

        /// <summary>
        /// Setup the component.
        /// </summary>
        void Setup();
        
        /// <summary>
        /// Processes event message.
        /// </summary>
        /// <param name="commsMessage">Comms message to process.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        Task ProcessAsync(CommsMessage commsMessage, CancellationToken cancellationToken);
    }
}