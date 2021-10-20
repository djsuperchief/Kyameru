﻿using Kyameru.Component.Ftp.Contracts;
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
            throw new NotImplementedException();
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            return new From(headers, new Components.WebRequestUtility());
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new To(headers, new Components.WebRequestUtility());
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFtpFrom, From>();
            return serviceCollection;
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFtpTo, To>();
            return serviceCollection;
        }
    }
}