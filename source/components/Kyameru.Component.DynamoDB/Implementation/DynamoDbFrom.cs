using System;
using System.Collections.Generic;
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
        private System.Threading.Timer _poller;
        private int _polltime;
        private bool _isStopping = false;
        private AutoResetEvent _timerEvent = new AutoResetEvent(false);
        private Dictionary<string, string> _headers;

        public void Setup()
        {
            ValidateHeaders();
            
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _poller = new Timer(TimerElapsed, _timerEvent, TimeSpan.FromSeconds(_polltime), TimeSpan.FromSeconds(_polltime));
            await Process(_cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _isStopping = true;
            await _poller.DisposeAsync();
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
        }
        
        private async void TimerElapsed(object state)
        {
            if (!_isStopping)
            {
                await Process(_cancellationToken);
            }

            await Task.CompletedTask;
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            var table = await _dynamoDbClient.DescribeTableAsync(_tableName, stoppingToken);
            var stream = table.Table.LatestStreamArn;

            var shards = await _dynamoDbStreams.DescribeStreamAsync(stream, stoppingToken);

            foreach (var shard in shards.StreamDescription.Shards)
            {
                await ProcessShard(shard, stream, stoppingToken);
            }
            
        }

        private async Task ProcessShard(Shard shard, string streamArn, CancellationToken stoppingToken)
        {
            var shardIteratorResponse = await _dynamoDbStreams.GetShardIteratorAsync(new GetShardIteratorRequest()
            {
                StreamArn = streamArn,
                ShardId = shard.ShardId,
                ShardIteratorType = ShardIteratorType.LATEST
            }, stoppingToken);
            
            var iterator = shardIteratorResponse.ShardIterator;
            var records = new List<string>();
            while (!string.IsNullOrWhiteSpace(iterator) && !stoppingToken.IsCancellationRequested)
            {
                var recordsResponse = await _dynamoDbStreams.GetRecordsAsync(iterator, stoppingToken);
                foreach (var record in recordsResponse.Records)
                {
                    records.Add(record.Dynamodb.Keys.ToJson());
                }

                iterator = recordsResponse.NextShardIterator;
            }

            var routable = new Routable(new Dictionary<string, string>(), records);
            if (OnActionAsync != null)
            {
                await OnActionAsync.Invoke(this, new RoutableEventData(routable,  stoppingToken));
            }
        }


        private void ValidateHeaders()
        {
            if (!_headers.TryGetValue("Host", out var header))
            {
                throw new MissingHeaderException(Resources.EXCEPTION_MISSINGTABLENAME);
            }
            
            _tableName = header;
        }
    }
}