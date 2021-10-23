using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Kyameru.Component.Slack
{
    /// <summary>
    /// Implementation of inflator.
    /// </summary>
    public class Inflator : IOasis
    {
        /// <summary>
        /// Creates an atomic component.
        /// </summary>
        /// <param name="headers">Headers to process.</param>
        /// <returns>Throws a not implemented exception</returns>
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>]
        /// <param name="isAtomic">value indicating whether the component is considered to be atomic.</param>
        /// <returns>Returns a new instance of a <see cref="IFromComponent"/> class.</returns>
        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IToComponent"/> class.</returns>
        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new SlackTo(headers);
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}