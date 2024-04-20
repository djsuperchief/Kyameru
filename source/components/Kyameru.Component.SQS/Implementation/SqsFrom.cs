using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;

namespace Kyameru.Component.SQS;

public class SqsFrom : IFrom
{
    public event EventHandler<Log>? OnLog;
    public event EventHandler<Routable>? OnAction;
    public event AsyncEventHandler<RoutableEventData>? OnActionAsync;
    public void Setup()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}