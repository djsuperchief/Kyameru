﻿using Kyameru.Component.Ftp.Contracts;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// Initiator class.
    /// </summary>
    public class Inflator : IOasis
    {
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "ATOMIC", "Ftp"));
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            return new From(headers, new Components.WebRequestUtility());
        }

        public IScheduleComponent CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new To(headers, new Components.WebRequestUtility());
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