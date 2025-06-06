﻿using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
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
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IFromChainLink"/> class.</returns>
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {

            return new FileWatcher(headers, new Utilities.BaseFileSystemWatcher());
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Incoming headers.</param>
        /// <returns>Returns a new instance of a <see cref="IToChainLink"/> class.</returns>
        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new FileTo(headers, new Utilities.FileUtils());
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}