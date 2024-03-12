using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Error component.
    /// </summary>
    public interface IErrorComponent : IComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        void Process(Routable item);

        /// <summary>
        /// Process the incoming request
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns an async task.</returns>
        Task ProcessAsync(Routable item, CancellationToken cancellationToken);
    }
}