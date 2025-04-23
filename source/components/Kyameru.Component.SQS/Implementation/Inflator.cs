using System;
using System.Collections.Generic;
using Amazon.SQS;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.Sqs;

public class Inflator : IOasis
{
    public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        var from = serviceProvider.GetRequiredService<IFrom>();
        from.SetHeaders(headers);
        return from;
    }

    public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        var to = serviceProvider.GetRequiredService<ITo>();
        to.SetHeaders(headers);
        return to;
    }

    public IAtomicLink CreateAtomicComponent(Dictionary<string, string> headers)
    {
        throw new RouteNotAvailableException(string.Format(Core.Resources.ERROR_ROUTE_UNAVAILABLE, "ATOMIC", "SQS"));
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddAWSService<IAmazonSQS>();
        serviceCollection.AddTransient<ITo, SqsTo>();
        return serviceCollection;
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddAWSService<IAmazonSQS>();
        serviceCollection.AddTransient<IFrom, SqsFrom>();
        return serviceCollection;
    }

    public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }
}