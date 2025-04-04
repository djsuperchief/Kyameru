using System;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Conditional route decision component.
    /// </summary>
    /// <remarks>This is used in conditions that are specified by configuration.</remarks>
    public interface IConditionalComponent
    {
        /// <summary>
        /// Executes the conditional requirements to indicate if the To component should execute.
        /// </summary>
        /// <param name="routable">Kyameru message.</param>
        /// <returns>Returns a <see cref="bool"/> indicating if the To component should execute.</returns>
        bool Execute(Routable routable);
    }
}
