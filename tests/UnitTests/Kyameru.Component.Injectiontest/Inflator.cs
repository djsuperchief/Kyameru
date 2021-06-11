using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Injectiontest
{
    public class Inflator : Kyameru.Core.Contracts.IOasis
    {
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
        {
            if(headers.ContainsKey("WillError"))
            {
                throw new NotImplementedException("from");
            }

            IMyFrom response = serviceProvider.GetRequiredService<IMyFrom>();
            response.AddHeaders(headers);
            return response;
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            if (headers.ContainsKey("WillError"))
            {
                throw new NotImplementedException("to");
            }

            IMyTo response = serviceProvider.GetRequiredService<IMyTo>();
            response.AddHeaders(headers);
            return response;
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            return serviceCollection.InstallToService();
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            return serviceCollection.InstallFromService();
        }
    }
}
