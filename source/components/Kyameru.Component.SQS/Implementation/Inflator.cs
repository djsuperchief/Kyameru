using System;
using System.Collections.Generic;
using Amazon.SQS;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.SQS;

public class Inflator : IOasis
{
    public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        var to = serviceProvider.GetRequiredService<ITo>();
        to.SetHeaders(headers);
        return to;
    }

    public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
    {
        throw new NotImplementedException();
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
}