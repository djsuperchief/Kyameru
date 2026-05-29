using System;
using System.Collections.Generic;
using Amazon.DynamoDBStreams;
using Amazon.DynamoDBv2;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kyameru.Component.DynamoDB
{
    public class Inflator : IOasis
    {
        public IFromChainLink CreateFromComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            var component = serviceProvider.GetRequiredService<IFrom>();
            component.SetHeaders(headers);
            return component;
        }

        public IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            var component = serviceProvider.GetRequiredService<ITo>();
            component.SetHeaders(headers);
            return component;
        }

        public IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddAWSService<IAmazonDynamoDB>();
            serviceCollection.AddTransient<ITo, DynamoDbTo>();
            serviceCollection.TryAddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
            return serviceCollection;
        }

        public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddAWSService<IAmazonDynamoDB>();
            serviceCollection.TryAddAWSService<IAmazonDynamoDBStreams>();
            serviceCollection.TryAddTransient<IFrom, DynamoDbFrom>();
            return serviceCollection;
        }

        public IServiceCollection RegisterScheduled(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        public void RegisterDependencies(IServiceCollection services, List<ChainLinkDependency> fromDependencies, List<ChainLinkDependency> toDependencies)
        {
            throw new NotImplementedException();
        }
    }
}