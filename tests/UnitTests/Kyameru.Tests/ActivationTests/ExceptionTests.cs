using Kyameru.Core.Contracts;
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

public class ExceptionTests : BaseTests
{

    [Fact]
    public async Task FromException()
    {
        Routable routable = null;
        var errorComponent = Substitute.For<IErrorProcessor>();
        var thread = TestThread.CreateDeferred(2);
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            thread.Continue();
            return Task.CompletedTask;
        });

        var generics = Component.Generic.Builder.Create()
            .WithFrom(() =>
            {
                throw new NotImplementedException();
            })
            .WithTo((x) => { });

        var builder = Route.From("generic:///error")
            .To("generic:///wontexecute")
            .Error(errorComponent);
        var service = BuildAndGetServices(builder, generics);
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Null(routable);
    }

    [Fact]
    public async Task FromRaiseException()
    {
        var errorComponent = Substitute.For<IErrorProcessor>();
        var generics = Component.Generic.Builder.Create()
            .WithFrom(() =>
            {
                throw new NotImplementedException();
            })
            .WithTo((x) => { });

        var builder = Route.From("generic:///error")
            .To("generic:///wontexecute")
            .Error(errorComponent)
            .RaiseExceptions();
        var service = BuildAndGetServices(builder, generics);

        await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await service.StartAsync(default);
        });
    }

    [Fact]
    public async Task ComponentError()
    {
        Routable routable = null;
        var thread = TestThread.CreateDeferred(2);
        var errorComponent = Substitute.For<IErrorProcessor>();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            thread.Continue();
            return Task.CompletedTask;
        });

        var processComponent = Substitute.For<IProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            throw new Kyameru.Core.Exceptions.ProcessException("Manual Error");
        });

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((x) => { });

        var builder = Route.From("generic:///ok")
            .Process(processComponent)
            .To("generic:///ok")
            .Error(errorComponent);

        var service = BuildAndGetServices(builder, generics);

        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "Processing component"));
    }

    [Fact]
    public async Task ToError()
    {
        Routable routable = null;

        var errorComponent = Substitute.For<IErrorProcessor>();
        var processComponent = Substitute.For<IProcessor>();
        var thread = TestThread.CreateDeferred(2);
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            thread.Continue();
            return Task.CompletedTask;
        });

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((x) =>
            {
                throw new NotImplementedException();
            });

        var builder = Route.From("generic:///ok")
            .Process(processComponent)
            .To("generic:///ok")
            .Error(errorComponent);


        var service = BuildAndGetServices(builder, generics);

        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.True(this.IsInError(routable, "To Component"));
    }

    [Fact]
    public async Task ErrorComponentErrors()
    {
        Routable routable = null;
        var errorComponent = Substitute.For<IErrorProcessor>();
        var processComponent = Substitute.For<IProcessor>();
        var thread = TestThread.CreateDeferred();
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            throw new ProcessException("Manual Error", new IndexOutOfRangeException("Random index"));
        });


        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((x) => { });

        var builder = Route.From("generic:///ok")
            .Process(processComponent)
            .To("generic:///ok")
            .Error(errorComponent);

        var service = BuildAndGetServices(builder, generics);

        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
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
}