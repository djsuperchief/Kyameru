using System;
using System.Dynamic;
using Kyameru.Core.Comms;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Generic;

public class Builder
{
    private Func<Routable> _fromProcessing;
    private Action<Routable> _toProcessing;
    private bool eventFrom = false;

#pragma warning disable CS8618
    private Builder()
    {

    }

#pragma warning restore CS8618

    public static Builder Create()
    {
        return new Builder();
    }

    public Builder WithFrom(Func<Routable> processing)
    {
        _fromProcessing = processing;
        return this;
    }

    public Builder WithFrom()
    {
        _fromProcessing = () => new Routable(new Dictionary<string, string> { { "FROM", "Executed" } }, "CanExecute");
        return this;
    }

    public Builder WithEventFrom()
    {
        eventFrom = true;
        _fromProcessing = () => new Routable(new Dictionary<string, string> { { "FROM", "Executed" } }, "CanExecute");
        return this;
    }

    public Builder WithTo(Action<Routable> processing)
    {
        _toProcessing = processing;
        return this;
    }

    public void Build(IServiceCollection services)
    {
        if (eventFrom)
        {
            services.AddTransient<IGenericEventFrom>(x => CreateFromEvent());
        }
        else
        {
            services.AddTransient<IGenericFrom>(x => CreateFrom());
        }

        services.AddTransient<IGenericTo>(x =>
        {
            return CreateTo();
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

    private IGenericEventFrom CreateFromEvent()
    {
        var response = Substitute.For<IGenericEventFrom>();
        response.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            var message = ((GenericMessage)x.Arg<CommsMessage>().Data).Info;
            var routable = new Routable(new Dictionary<string, string> { { "EventFrom", "Executed" } },
                message);
            var routableData = new RoutableEventData(routable, x.Arg<CancellationToken>());
            response.OnActionAsync += Raise.Event<AsyncEventHandler<RoutableEventData>>(null, routableData);
            return Task.CompletedTask;
        });

        return response;
    }
}
