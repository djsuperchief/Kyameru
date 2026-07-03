using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Kyameru.Component.Dynamodb.Contracts;
using Kyameru.Component.Dynamodb.Entities;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Dynamodb
{
    public class DynamoDbTo : ITo
    {
        private readonly IDynamoDbUpserter _dbUpserter;
        private bool processBatch = false;
        private string tableName;

        public DynamoDbTo(IDynamoDbUpserter dbUpserter)
        {
            _dbUpserter = dbUpserter;
        }
        
        public event EventHandler<Log>? OnLog;
        
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            Log(LogLevel.Information, Resources.INFO_PROCESSING);
            ValidateRoutable(routable);
            var overrideTable = routable.Headers.TryGetValue("DynamoDbOverrideTable", tableName);
            if (string.IsNullOrWhiteSpace(overrideTable))
            {
                throw new Exceptions.MissingDynamoTable(Resources.EXCEPTION_MISSINGTABLENAME);
            }
            
            processBatch = routable.Body is IEnumerable;
            
            if (processBatch)
            {
                Log(LogLevel.Information, Resources.INFO_PROCESSINGSINGLE);
                await _dbUpserter.SaveAsync(((IEnumerable)routable.Body).Cast<IDynamoRecord>().ToList(), overrideTable, cancellationToken);
            }
            else
            {
                Log(LogLevel.Information, Resources.INFO_PROCESSINGMULTIPLE);
                await _dbUpserter.SaveAsync(routable.Body! as IDynamoRecord, overrideTable, cancellationToken);
            }
            
            await Task.CompletedTask;
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Host", out var header))
            {
                throw new MissingHeaderException(Resources.EXCEPTION_MISSINGTABLENAME);
            }
            
            tableName = header;
        }

        private void ValidateRoutable(Routable routable)
        {
            Log(LogLevel.Debug, Resources.DEBUG_VALIDATINGROUTABLE);
            if (routable == null)
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLEEMPTY);
            }

            if (!(routable.Body is IDynamoRecord) && !(routable.Body is IEnumerable<IDynamoRecord>))
            {
                throw new Exceptions.InvalidTypeException(Resources.EXCEPTION_ROUTABLENOTCOMPATIBLE);
            }
        }

        private void Log(LogLevel logLevel, string message, Exception? innerException = null)
        {
            OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, innerException));
        }
    }
}