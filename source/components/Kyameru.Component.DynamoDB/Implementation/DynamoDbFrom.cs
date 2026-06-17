using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBStreams;
using Amazon.DynamoDBStreams.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.Extensions;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Sys;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.DynamoDB
{
    public class DynamoDbFrom : IFrom
    {
        public DynamoDbFrom(IAmazonDynamoDB dynamoDbClient, IAmazonDynamoDBStreams dynamoDbStreams)
        {
            _dynamoDbClient = dynamoDbClient;
            _dynamoDbStreams = dynamoDbStreams;
        }
        
        public event EventHandler<Log>? OnLog;
        public event AsyncEventHandler<RoutableEventData>? OnActionAsync;
        private string _tableName;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IAmazonDynamoDBStreams _dynamoDbStreams;
        private CancellationToken _cancellationToken;
        private int _polltime;
        private bool _isStopping = false;
        private Dictionary<string, string> _headers;

        public void Setup()
        {
            
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            await Process(_cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _isStopping = true;
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            ValidateHeaders();
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            try
            {
                var table = await _dynamoDbClient.DescribeTableAsync(_tableName, stoppingToken);
                var stream = table.Table.LatestStreamArn;
                Log(LogLevel.Information, string.Format(Resources.INFO_PROCESSINGTABLE, _tableName));

                var shards = await _dynamoDbStreams.DescribeStreamAsync(stream, stoppingToken);

                var tasks = shards.StreamDescription.Shards.Select(shard => ProcessShard(shard, stream, stoppingToken));
                
                await Task.WhenAll(tasks);
            }
            catch (TaskCanceledException)
            {
                Log(LogLevel.Information, Resources.INFO_PROCESSINGTERMINATED);
            }
        }

        private async Task ProcessShard(Shard shard, string streamArn, CancellationToken stoppingToken)
        {
            try
            {
                var iterator = string.Empty;
                while (!stoppingToken.IsCancellationRequested)
                {
                    var records = new List<string>();
                    if (string.IsNullOrWhiteSpace(iterator))
                    {
                        var shardIterator = await GetNextShardIterator(shard, streamArn, stoppingToken);
                        iterator = shardIterator.ShardIterator;
                    }

                    var recordsResponse = await _dynamoDbStreams.GetRecordsAsync(iterator, stoppingToken);
                    foreach (var record in recordsResponse.Records)
                    {
                        records.Add(record.Dynamodb.NewImage.ToJson());
                    }

                    if (records.Any())
                    {
                        var routable = new Routable(new Dictionary<string, string>(), records);
                        if (OnActionAsync != null)
                        {
                            await OnActionAsync.Invoke(this, new RoutableEventData(routable, stoppingToken));
                        }
                    }

                    iterator = recordsResponse.NextShardIterator;

                    await Task.Delay(_polltime, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                
                Log(LogLevel.Information, Resources.INFO_PROCESSINGTERMINATED);
            }
        }

        private async Task<GetShardIteratorResponse> GetNextShardIterator(Shard shard, string streamArn,
            CancellationToken stoppingToken) => await _dynamoDbStreams.GetShardIteratorAsync(
            new GetShardIteratorRequest()
            {
                StreamArn = streamArn,
                ShardId = shard.ShardId,
                ShardIteratorType = ShardIteratorType.LATEST
            }, stoppingToken);


        private void ValidateHeaders()
        {
            if (!_headers.TryGetValue("Host", out var header))
            {
                throw new MissingHeaderException(Resources.EXCEPTION_MISSINGTABLENAME);
            }
            
            _tableName = header;
            _polltime = 6;
            if (_headers.TryGetValue("PollTime", out var pollTimeValue))
            {
                _polltime = int.Parse(pollTimeValue) * 1000;
            }
            
        }

        private void Log(LogLevel logLevel, string message, Exception? exception = null)
        {
            if (OnLog != null)
            {
                OnLog(this, new Log(logLevel, message, exception));
            }
        }
    }
}