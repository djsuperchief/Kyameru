using System;
using System.Collections.Generic;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            throw new NotImplementedException();
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            throw new NotImplementedException();
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider, Guid id)
        {
            var toChain = serviceProvider.GetRequiredService<IRestTo>();
            toChain.SetHeaders(headers);

            return toChain;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection, Guid id)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection, Guid id)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection, Guid id)
        {
            serviceCollection.AddTransient<IRestTo, RestTo>();
            return serviceCollection;
        }
    }
}