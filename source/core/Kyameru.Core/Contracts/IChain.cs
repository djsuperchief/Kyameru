using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Core processing component interface.
    /// </summary>
    /// <typeparam name="T">Type of processing class.</typeparam>
    public interface IChain<T> where T : class
    {
        /// <summary>
        /// Sets the next processing component.
        /// </summary>
        /// <param name="next">Next component.</param>
        /// <returns>Returns an instance of the <see cref="IChain{T}"/> interface.</returns>
        IChain<T> SetNext(IChain<T> next);

        /// <summary>
        /// Triggers the next component in the chain.
        /// </summary>
        /// <param name="item">Processing Component.</param>
        void Handle(T item);

        /// <summary>
        /// Triggers the next component in the chain.
        /// </summary>
        /// <param name="item">Processing Component.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        Task HandleAsync(T item, CancellationToken cancellationToken);
    }
}