using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Kyameru.Core.Comms;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Activator interface for components utilising event driven triggers.
    /// </summary>
    public interface IEventOasis : IOasis
    {
        /// <summary>
        /// Creates a from component triggered by an event.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        IFromEventChainLink CreateFromEvent(Dictionary<string, string> headers, IServiceProvider serviceProvider);
    }
}