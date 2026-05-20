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
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.DynamoDB
{
    public class DynamoDbUpserter : IDynamoDbUpserter
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbUpserter> _logger;
        private readonly IAmazonDynamoDB _client;

        public DynamoDbUpserter(IDynamoDBContext dbContext, IAmazonDynamoDB client, ILogger<DynamoDbUpserter> logger)
        {
            _dynamoDbContext = dbContext;
            _logger = logger;
            _client = client;
        }
        
        public async Task SaveAsync(object entity, string table = "", CancellationToken cancellationToken = default)
        {

            var attributeMap = GenerateAttributeMap(entity);
            var request = new PutItemRequest()
            {
                TableName = table,
                Item = attributeMap
            };
            
            await _client.PutItemAsync(request, cancellationToken);

        }

        public async Task SaveAsync(IEnumerable<object>? entities, string table, CancellationToken cancellationToken = default)
        {
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
        }

        private Dictionary<string, AttributeValue> GenerateAttributeMap(object entity)
        {
            var doc = Document.FromJson(JsonSerializer.Serialize(entity));
            return doc.ToAttributeMap();
        }

        private List<List<object>> GenerateBatches(List<object> records, int batchSize) =>
            records.Select((item, index) => new { item, index })
                .GroupBy(x => x.index / batchSize)
                .Select(group => group.Select(x => x.item).ToList())
                .ToList();

        private bool ValidateBatchSize(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new InvalidBatchSizeException(Resources.EXCEPTION_INVALIDBATCHSIZE_SMALL);
            }

            if (batchSize > 25)
            {
                throw new InvalidBatchSizeException(Resources.EXCEPTION_INVALIDBATCHSIZE_LARGE);
            }

            return true;
        }

        private SaveConfig? GetSaveConfig(string tableOverride)
        {
            SaveConfig saveConfig = null;
            if (!string.IsNullOrWhiteSpace(tableOverride))
            {
                saveConfig = new SaveConfig()
                {
                    OverrideTableName = tableOverride,
                };
            }

            return saveConfig;
        }

        private BatchWriteConfig? GetBatchWriteConfig(string tableOverride)
        {
            BatchWriteConfig? saveConfig = null;
            if (!string.IsNullOrWhiteSpace(tableOverride))
            {
                saveConfig = new BatchWriteConfig()
                {
                    OverrideTableName = tableOverride,
                };
            }

            return saveConfig;
        }
    }
}