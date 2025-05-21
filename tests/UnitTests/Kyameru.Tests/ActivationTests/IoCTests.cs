using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Tests;
using Kyameru.Tests.Extensions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kyameru.Facts.ActivationFacts;

public class IoCFacts : BaseTests
{
    [Fact]
    public void CanSetupFullFact()
    {
        var serviceCollection = GetServiceDescriptors();
        var processComponent = Substitute.For<IProcessor>();
        var errorComponent = Substitute.For<IErrorProcessor>();
        Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo(x => { })
            .Build(serviceCollection);
        Kyameru.Route.From($"generic://hello?TestName=Setup")
            .Process(processComponent)
            .Process(processComponent)
            .To("generic://world")
            .To("generic://kyameru")
            .Error(errorComponent)
            .Id("WillNotExecute")
            .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<IHostedService>());
    }

    [Fact]
    public async Task CanExecute()
    {
        // Checks that From, Component and To ran.
        var thread = TestThread.CreateDeferred(10);
        var routable = new Routable(new Dictionary<string, string>(), string.Empty);
        var expected = new List<string>()
        {
            { "FROM:Executed" },
            { "PROCESSOR:Executed" },
            { "TO:Executed" }
        };

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((Routable x) =>
            {
                x.SetHeader("TO", "Executed");
                routable = x;
                thread.Continue();
            });

        var builder = Route.From("generic:///CanExecute")
            .Process(GetProcessor())
            .To("generic:///CanExecute");

        var service = BuildAndGetServices(builder, generics);

        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        var result = routable.Headers.ToAssertable().Where(x => expected.Contains(x));
        Assert.Equal(expected, result);

    }

    [Fact]
    public async Task CanRunDIComponent()
    {
        Routable routable = null;
        var services = GetServiceDescriptors();
        var thread = TestThread.CreateDeferred(10);
        var expected = new List<string>()
        {
            { "ComponentRan:Yes" },
            { "FROM:Executed" },
            { "TO:Executed"}
        };

        Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((Routable x) =>
            {
                x.SetHeader("TO", "Executed");
                routable = x;
                thread.Continue();
            })
            .Build(services);

        Route.From("generic:///CanExecute")
            .Process<Tests.Mocks.IMyComponent>()
            .To("generic:///CanExecute")
            .Build(services);
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();
        Assert.Equal(expected, routable.Headers.ToAssertable());
    }

    [Fact]
    public async Task CanExecuteMultipleChains()
    {
        Routable routable = null;
        var thread = TestThread.CreateDeferred();
        var processComponent = Substitute.For<IProcessor>();
        var secondComponent = Substitute.For<IProcessor>();

        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("ComponentOne", "Executed");
            return Task.CompletedTask;
        });

        secondComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("ComponentTwo", "Executed");
            return Task.CompletedTask;
        });

        var expected = new List<string>()
        {
            { "ComponentOne:Executed" },
            { "ComponentTwo:Executed" },
            { "FROM:Executed" },
            { "TO:Executed" },
            { "TOSECOND:Executed" }
        };

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo((Routable x) =>
            {
                var threadContinue = false;
                if (x.Headers.ContainsKey("TO"))
                {
                    x.SetHeader("TOSECOND", "Executed");
                    threadContinue = true;
                }
                else
                {
                    x.SetHeader("TO", "Executed");
                }

                routable = x;
                if (threadContinue)
                {
                    thread.Continue();
                }
            });

        var builder = Route.From("generic:///start")
            .Process(processComponent)
            .Process(secondComponent)
            .To("generic:///first")
            .To("generic:///second");

        var service = BuildAndGetServices(builder, generics);
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();
        Assert.Equal(expected, routable.Headers.ToAssertable());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddHeaderErrors(bool secondFunction)
    {
        var expected = new List<string>() {
            { "FROM:Executed" }
        };
        Routable routable = null;
        var thread = TestThread.CreateDeferred();
        var error = Substitute.For<IErrorProcessor>();
        error.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            routable = x.Arg<Routable>();
            thread.Continue();
            return Task.CompletedTask;
        });

        var generics = Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo(x => { });

        Core.Builder builder = null;

        builder = Route.From("generic:///test")
            .AddHeader("Error", () =>
            {
                throw new NotImplementedException();
            })
            .To("generic:///shouldNotBeHere")
            .Error(error);

        if (secondFunction)
        {
            builder = Route.From("generic:///test")
                .AddHeader("Error", (x) =>
                {
                    throw new NotImplementedException();
                })
                .To("generic:///shouldNotBeHere")
                .Error(error);
        }


        var service = BuildAndGetServices(builder, generics);
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal(expected, routable.Headers.ToAssertable());
    }

    [Fact]
    public async Task MultipleRoutesWork()
    {
        var calls = 0;
        var processComponent = Substitute.For<IProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            calls++;
            return Task.CompletedTask;
        });

        var threadOne = TestThread.CreateDeferred();
        var threadTwo = TestThread.CreateDeferred();

        var serviceCollection = GetServiceDescriptors();
        // this executes twice!
        Component.Generic.Builder.Create()
            .WithFrom()
            .WithTo(x => { })
            .Build(serviceCollection);

        Kyameru.Route.From("generic://first?TestName=TwoRoutes")
                .Process(processComponent)
                .To("generic://world", x =>
                {
                    threadOne.Continue();
                })
                .Id("First")
                .Build(serviceCollection);

        Kyameru.Route.From("generic://second?TestName=TwoRoutes")
                .Process(processComponent)
                .To("generic://world", x =>
                {
                    threadTwo.Continue();
                })
                .Id("Second")
                .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var services = provider.GetServices<IHostedService>();

        Assert.Equal(2, services.Count());
        threadOne.SetThread(services.ElementAt(0).StartAsync);
        threadTwo.SetThread(services.ElementAt(1).StartAsync);
        threadOne.StartAndWait();
        threadTwo.StartAndWait();
        await threadOne.CancelAsync();
        await threadTwo.CancelAsync();

        Assert.Equal(2, calls);
    }

    #region Helpers

    private IProcessor GetProcessor()
    {
        var processor = Substitute.For<IProcessor>();
        processor.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("PROCESSOR", "Executed");
            return Task.CompletedTask;
        });

        return processor;
    }

    #endregion Helpers
}