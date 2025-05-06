using System;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Generic;

public class Builder
{
    private Func<Routable> _fromProcessing;
    private Action<Routable> _toProcessing;

    protected Builder()
    {

    }
    public static Builder Create()
    {
        return new Builder();
    }

    public Builder WithFrom(Func<Routable> processing)
    {
        _fromProcessing = processing;
        return this;
    }

    public Builder WithTo(Action<Routable> processing)
    {
        _toProcessing = processing;
        return this;
    }

    public void Build(IServiceCollection services)
    {
        var from = CreateFrom();
        var to = CreateTo();
        services.AddTransient<IGenericFrom>(x =>
        {
            return from;
        });

        services.AddTransient<IGenericTo>(x =>
        {
            return to;
        });
    }

    private IGenericTo CreateTo()
    {
        var response = Substitute.For<IGenericTo>();
        response.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            _toProcessing.Invoke(x.Arg<Routable>());
            return Task.CompletedTask;
        });

        return response;
    }

    private IGenericFrom CreateFrom()
    {
        var response = Substitute.For<IGenericFrom>();
        response.StartAsync(default).ReturnsForAnyArgs(x =>
        {
            var routable = _fromProcessing.Invoke();
            var routableData = new RoutableEventData(routable, x.Arg<CancellationToken>());
            response.OnActionAsync += Raise.Event<AsyncEventHandler<RoutableEventData>>(null, routableData);
            return Task.CompletedTask;
        });

        return response;
    }
}
