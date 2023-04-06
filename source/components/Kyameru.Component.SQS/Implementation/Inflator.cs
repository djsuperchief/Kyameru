using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.SQS
{
    public class Inflator : IOasis
    {
        public Inflator()
        {
        }

        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }


        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
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

