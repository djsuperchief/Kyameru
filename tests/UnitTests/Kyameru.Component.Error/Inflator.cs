﻿using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public class Inflator : IOasis
    {
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            return new Atomic(headers);
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            return new From(headers);
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            return new To(headers);
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IScheduleComponent CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            throw new NotSupportedException();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}