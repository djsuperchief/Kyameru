using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Error;
using Kyameru.Component.Generic;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

public class DiTests
{
    [Fact]
    public async Task DependencyInjectionDoesNotError()
    {
        var routable = new Routable(new Dictionary<string, string>(), null);
        var thread = TestThread.CreateDeferred(2);
        var services = GetServiceDescriptors();
        var from = Substitute.For<IGenericFrom>();
        from.StartAsync(default).ReturnsForAnyArgs(x =>
        {
            var routableData = new RoutableEventData(new Routable(new Dictionary<string, string>(), "Test"), x.Arg<CancellationToken>());
            from.OnActionAsync += Raise.Event<AsyncEventHandler<RoutableEventData>>(routableData);
            return Task.CompletedTask;
        });

        var to = Substitute.For<IGenericTo>();
        to.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            return Task.CompletedTask;
        });

        var atomic = Substitute.For<IGenericAtomic>();
        atomic.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetBody<string>("Atomic Done");
            routable = x.Arg<Routable>();
            thread.Continue();
            return Task.CompletedTask;
        });


        Route.From("generic:///test")
        .To("generic:///test")
        .Atomic("generic:///test")
        .Id("dynamic-component")
        .Build(services);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("Atomic Done", routable.Body);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });


        return serviceCollection;
    }


}
