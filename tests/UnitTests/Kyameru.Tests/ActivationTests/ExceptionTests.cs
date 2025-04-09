﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

public class ExceptionTests
{

    [Fact]
    public async Task FromException()
    {
        Routable routable = null;
        var errorComponent = Substitute.For<IErrorComponent>();
        var processComponent = Substitute.For<IProcessComponent>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        var service = this.GetHostedService(SetupChain, processComponent, errorComponent, true);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.Null(routable);
    }

    [Fact]
    public async Task FromRaiseException()
    {
        var errorComponent = Substitute.For<IErrorComponent>();
        var processComponent = Substitute.For<IProcessComponent>();
        await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            var service = this.GetHostedService(SetupBubbleChain, processComponent, errorComponent, true);
            await service.StartAsync(CancellationToken.None);
        });
    }

    [Fact]
    public async Task ComponentError()
    {
        Routable routable = null;

        var errorComponent = Substitute.For<IErrorComponent>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        var processComponent = Substitute.For<IProcessComponent>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            throw new Kyameru.Core.Exceptions.ProcessException("Manual Error");
        });

        var service = this.GetHostedService(SetupChain, processComponent, errorComponent);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "Processing component"));
    }

    [Fact]
    public async Task ToError()
    {
        Routable routable = null;

        var errorComponent = Substitute.For<IErrorComponent>();
        var processComponent = Substitute.For<IProcessComponent>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        var service = this.GetHostedService(SetupChain, processComponent, errorComponent, false, true);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "To Component"));
    }

    [Fact]
    public async Task AtomicError()
    {
        Routable routable = null;
        var errorComponent = Substitute.For<IErrorComponent>();
        var processComponent = Substitute.For<IProcessComponent>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        var service = this.GetHostedService(SetupChain, processComponent, errorComponent, false, false, true);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "Atomic Component"));
    }

    [Fact]
    public async Task ErrorComponentErrors()
    {
        Routable routable = null;
        var errorComponent = Substitute.For<IErrorComponent>();
        var processComponent = Substitute.For<IProcessComponent>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            throw new ProcessException("Manual Error", new IndexOutOfRangeException("Random index"));
        });

        var service = this.GetHostedService(SetupChain, processComponent, errorComponent, false, false, true);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "Error Component"));
    }

    private bool IsInError(Routable routable, string component)
    {
        return routable != null
            && routable.Error.Component == component
            && routable.Error.CurrentAction == "Handle"
            && routable.Error.Message == "Manual Error";
    }

    private IHostedService GetHostedService(
        Action<string, string, string, IServiceCollection, IProcessComponent, IErrorComponent> setup,
        IProcessComponent processComponent,
        IErrorComponent errorComponent,
        bool fromError = false,
        bool toError = false,
        bool atomicError = false)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });
        var from = $"error://path?Error={fromError}";
        var to = $"error://path:test@test.com?Error={toError}";
        var atomic = $"error://path?Error={atomicError}";

        setup.Invoke(from, to, atomic, serviceCollection, processComponent, errorComponent);

        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

    private void SetupChain(string from, string to, string atomic, IServiceCollection serviceDescriptors, IProcessComponent processComponent, IErrorComponent errorComponent)
    {
        Kyameru.Route.From(from)
            .Process(processComponent)
            .To(to)
            .Atomic(atomic)
            .Error(errorComponent)
            .Build(serviceDescriptors);
    }

    private void SetupBubbleChain(string from, string to, string atomic, IServiceCollection serviceDescriptors, IProcessComponent processComponent, IErrorComponent errorComponent)
    {
        Kyameru.Route.From(from)
            .Process(processComponent)
            .To(to)
            .Atomic(atomic)
            .Error(errorComponent)
            .RaiseExceptions()
            .Build(serviceDescriptors);
    }
}