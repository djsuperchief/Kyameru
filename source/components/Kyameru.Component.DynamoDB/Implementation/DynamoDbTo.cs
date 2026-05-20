using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Entities;
using Kyameru.Core.Entities;

namespace Kyameru.Component.DynamoDB
{
    public class DynamoDbTo : ITo
    {
        private readonly IDynamoDbUpserter _dbUpserter;
        private bool processBatch = false;

        public DynamoDbTo(IDynamoDbUpserter dbUpserter)
        {
            _dbUpserter = dbUpserter;
        }
        
        public event EventHandler<Log>? OnLog;
        
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            //ValidateRoutable(routable);
            var overrideTable = routable.Headers.TryGetValue("DynamoDbOverrideTable", string.Empty);
            processBatch = routable.Body is IEnumerable;
            
            if (processBatch)
            {
                await _dbUpserter.SaveAsync(((IEnumerable)routable.Body).Cast<IDynamoRecord>().ToList(), overrideTable, cancellationToken);
            }
            else
            {
                await _dbUpserter.SaveAsync(routable.Body! as IDynamoRecord, overrideTable, cancellationToken);
            }
            
            await Task.CompletedTask;
        }

        private void ValidateRoutable(Routable routable)
        {
            if (routable == null)
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLEEMPTY);
            }

            if (!(routable.Body is IDynamoRecord) && !(routable.Body is IEnumerable))
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLENOTCOMPATIBLE);
            }
        }
    }
}