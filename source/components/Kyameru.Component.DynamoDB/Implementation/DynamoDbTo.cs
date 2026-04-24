using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.DynamoDB
{
    public class DynamoDbTo : ITo
    {
        private readonly IDynamoDBContext _dynamoDbClient;

        public DynamoDbTo(IDynamoDBContext dynamoDb)
        {
            _dynamoDbClient = dynamoDb;
        }
        
        public event EventHandler<Log>? OnLog;
        
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            ValidateRoutable(routable);

            await Task.CompletedTask;
        }

        private void ValidateRoutable(Routable routable)
        {
            if (routable == null)
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLEEMPTY);
            }

            if (!(routable.Body is IDynamoTable))
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLENOTCOMPATIBLE);
            }
        }
    }
}