using System;
using System.Collections.Generic;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.Rest
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            var toChain = serviceProvider.GetRequiredService<IRestTo>();
            toChain.SetHeaders(headers);
            return toChain;
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IHttpContentFactory, HttpContentFactory>();
            serviceCollection.TryAddTransient<IRestTo, RestTo>();
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IHttpContentFactory, HttpContentFactory>();
            serviceCollection.TryAddTransient<IRestFrom, RestFrom>();
            return serviceCollection;
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }
    }
}