using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Error component.
    /// </summary>
    public interface IErrorComponent : IProcessComponent
    {
        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="item">Message to be processed.</param>
        //void Process(Routable item);
    }
}