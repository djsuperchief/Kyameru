using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru
{
    /// <summary>
    /// Intermediary Processor
    /// </summary>
    public interface IProcessor : IComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="routable">Message to be processed.</param>
        /// <param name="cancellationToken">Thread cancellation token</param>
        Task ProcessAsync(Routable routable, CancellationToken cancellationToken);
    }
}