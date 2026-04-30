using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.DynamoDB
{
    public class DynamoDbUpserter : IDynamoDbUpserter
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbUpserter> _logger;

        public DynamoDbUpserter(IDynamoDBContext dbContext, ILogger<DynamoDbUpserter> logger)
        {
            _dynamoDbContext = dbContext;
            _logger = logger;
        }
        
        public async Task SaveAsync(IDynamoRecord entity, string tableOverride = "", CancellationToken cancellationToken = default)
        {
            var saveConfig = GetSaveConfig(tableOverride);
            await _dynamoDbContext.SaveAsync(entity, saveConfig, cancellationToken);
        }

        public async Task SaveAsync(IEnumerable<IDynamoRecord>? entities, string tableOverride = "", int batchSize = 25, CancellationToken cancellationToken = default)
        {
            var saveConfig = GetBatchWriteConfig(tableOverride);
            if (entities != null)
            {
                var batches = GenerateBatches(entities.ToList(), batchSize);
                foreach (var recordBatch in batches)
                {
                    var writeBatch = _dynamoDbContext.CreateBatchWrite<IDynamoRecord>(saveConfig);
                    writeBatch.AddPutItems(recordBatch);
                }
            }
            else
            {
                _logger.LogWarning(Resources.WARNING_NOENTITIES);
                await Task.CompletedTask;
            }
        }

        private List<List<IDynamoRecord>> GenerateBatches(List<IDynamoRecord> records, int batchSize) =>
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