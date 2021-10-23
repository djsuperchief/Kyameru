using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.File
{
    /// <summary>
    /// Implementation of inflator.
    /// </summary>
    public class Inflator : IOasis
    {
        /// <summary>
        /// Create an atomic component.
        /// </summary>
        /// <param name="headers">Incoming headers</param>
        /// <returns>Returns an instance of </returns>
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IFromComponent"/> class.</returns>
        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {

            return new FileWatcher(headers, new Utilities.BaseFileSystemWatcher());
        }

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IToComponent"/> class.</returns>
        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new FileTo(headers, new Utilities.FileUtils());
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}