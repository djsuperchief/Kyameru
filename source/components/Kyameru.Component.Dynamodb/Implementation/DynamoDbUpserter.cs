using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Kyameru.Component.Dynamodb.Contracts;
using Kyameru.Component.Dynamodb.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Dynamodb
{
    public class DynamoDbUpserter : IDynamoDbUpserter
    {
        private readonly ILogger<DynamoDbUpserter> _logger;
        private readonly IAmazonDynamoDB _client;

        public DynamoDbUpserter(IAmazonDynamoDB client, ILogger<DynamoDbUpserter> logger)
        {
            _logger = logger;
            _client = client;
        }
        
        public async Task SaveAsync(object entity, string table = "", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(Resources.INFO_PROCESSINGSINGLE);
            var attributeMap = GenerateAttributeMap(entity);
            var request = new PutItemRequest()
            {
                TableName = table,
                Item = attributeMap
            };
            await _client.PutItemAsync(request, cancellationToken);

            _logger.LogInformation(Resources.INFO_PROCESSINGCOMPLETE);
        }

        public async Task SaveAsync(IEnumerable<object>? entities, string table, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(Resources.INFO_PROCESSINGMULTIPLE);
            if (entities != null)
            {
                var writeRequests = entities.Select(x => new WriteRequest()
                {
                    PutRequest = new PutRequest()
                    {
                        Item = GenerateAttributeMap(x),
                    }
                }).ToList();

                var batchRequest = new BatchWriteItemRequest()
                {
                    RequestItems = new Dictionary<string, List<WriteRequest>>()
                    {
                        { table, writeRequests }
                    }
                };
                
                await _client.BatchWriteItemAsync(batchRequest, cancellationToken);
            }
            else
            {
                _logger.LogWarning(Resources.WARNING_NOENTITIES);
                await Task.CompletedTask;
            }
            
            _logger.LogInformation(Resources.INFO_PROCESSINGCOMPLETE);
        }

        private Dictionary<string, AttributeValue> GenerateAttributeMap(object entity)
        {
            _logger.LogDebug("Generating attribute map");
            var doc = Document.FromJson(JsonSerializer.Serialize(entity));
            return doc.ToAttributeMap();
        }
    }
}