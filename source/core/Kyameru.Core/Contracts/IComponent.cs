using Kyameru.Core.Entities;
using System;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Base component.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Event raised when a component logs.
        /// </summary>
        event EventHandler<Log> OnLog;
    }
}