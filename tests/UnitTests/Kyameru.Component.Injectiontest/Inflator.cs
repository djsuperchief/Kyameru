using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Injectiontest
{
    public class Inflator : Kyameru.Core.Contracts.IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            if (headers.ContainsKey("WillError"))
            {
                throw new NotImplementedException("from");
            }

            IMyFrom response = serviceProvider.GetRequiredService<IMyFrom>();
            response.AddHeaders(headers);
            return response;
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            if (headers.ContainsKey("WillError"))
            {
                throw new NotImplementedException("to");
            }

            IMyTo response = serviceProvider.GetRequiredService<IMyTo>();
            response.AddHeaders(headers);
            return response;
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection, Guid id)
        {
            return serviceCollection.InstallToService();
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection, Guid id)
        {
            return serviceCollection.InstallFromService();
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
