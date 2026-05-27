using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Amazon.DynamoDBStreams;
using Amazon.DynamoDBv2;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Core.Entities;
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
        private Dictionary<string, string> _headers;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IAmazonDynamoDBStreams _dynamoDbStreams;
        private CancellationToken _cancellationToken;
        private System.Timers.Timer _poller;
        private int _polltime;
        private bool _isStopping = false;

        public void Setup()
        {
            // TODO: validate headers.
            _poller.Elapsed += Poller_Elapsed;
            _poller.Interval = _polltime;
            _poller.AutoReset = true;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _poller.Start();
            await Process(_cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _isStopping = true;
            _poller.Stop();
            _poller.Elapsed -= Poller_Elapsed;
            
            await Task.CompletedTask;
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
        }
        
        private void Poller_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isStopping)
            {
                // process
            }
            else
            {
                _poller.Stop();
            }
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            
        }
    }
}