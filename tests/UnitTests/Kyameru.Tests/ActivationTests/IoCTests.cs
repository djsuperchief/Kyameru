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
    private readonly Dictionary<string, int> _callPoints = new Dictionary<string, int>();

    public IoCFacts()
    {
        _callPoints.Add("FROM", 1);
        _callPoints.Add("TO", 2);
        _callPoints.Add("ATOMIC", 3);
        _callPoints.Add("COMPONENT", 4);
        _callPoints.Add("ERROR", 5);
    }

    [Fact]
    public void CanSetupFullFact()
    {
        Assert.NotNull(AddComponent("CanSetupFullFact"));
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

    /// <summary>
    /// Obsolete, Atomic will be going.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CanExecuteAtomic()
    {
        var service = GetNoErrorChain("CanExecuteAtomic");
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();
        Assert.Equal(6, GetCallCount("CanExecuteAtomic"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddHeaderErrors(bool secondFunction)
    {
        var testName = $"AddHeaderErrors_{secondFunction.ToString()}";
        Component.Test.GlobalCalls.Clear(testName);
        var service = GetHeaderError(secondFunction, testName);
        var thread = TestThread.CreateNew(service.StartAsync, 2);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();
        Assert.Equal(1, GetCallCount(testName));
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

        var services = AddTwoRoutes(processComponent);
        Assert.Equal(2, services.Count());
        for (int i = 0; i < services.Count(); i++)
        {
            var thread = TestThread.CreateNew(services.ElementAt(i).StartAsync, 2);
            thread.Start();
            thread.WaitForExecution();
            await thread.CancelAsync();
        }

        Assert.Equal(2, calls);
    }

    #region Helpers

    private int GetCallCount(string test)
    {
        return Component.Test.GlobalCalls.CallDict[test].Sum(x => _callPoints[x]);
    }

    private IHostedService AddComponent(string test, bool multiChain = false)
    {
        var serviceCollection = GetServiceDescriptors();
        var processComponent = Substitute.For<IProcessor>();
        var errorComponent = Substitute.For<IErrorProcessor>();
        processComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            Kyameru.Component.Test.GlobalCalls.AddCall(test, "COMPONENT");
            return Task.CompletedTask;
        });
        errorComponent.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            Kyameru.Component.Test.GlobalCalls.AddCall(test, "ERROR");
            return Task.CompletedTask;
        });


        if (multiChain)
        {
            Kyameru.Route.From($"Test://hello?TestName={test}")
                .Process(processComponent)
                .Process(processComponent)
                .To("Test://world")
                .To("Test://kyameru")
                .Error(errorComponent)
                .Id("WillNotExecute")
                .Build(serviceCollection);
        }
        else
        {
            Kyameru.Route.From($"Test://hello?TestName={test}")
                .Process(processComponent)
                .To("Test://world")
                .Build(serviceCollection);
        }
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

    private IEnumerable<IHostedService> AddTwoRoutes(IProcessor processComponent)
    {
        var serviceCollection = GetServiceDescriptors();
        Kyameru.Route.From("Test://first?TestName=TwoRoutes")
                .Process(processComponent)
                .To("Test://world")
                .Build(serviceCollection);

        Kyameru.Route.From("Test://second?TestName=TwoRoutes")
                .Process(processComponent)
                .To("Test://world")
                .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetServices<IHostedService>();
    }

    private IHostedService SetupDIComponent(IProcessor diProcessor)
    {
        var serviceCollection = GetServiceDescriptors();
        Kyameru.Route.From("Test://hello?TestName=DITest")
            .Process<Tests.Mocks.IMyComponent>()
            .Process(diProcessor)
            .To("Test://world")
            .Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

    private IHostedService GetNoErrorChain(string test)
    {
        var serviceCollection = GetServiceDescriptors();
        Kyameru.Route.From($"Test://hello?TestName={test}")
            .To("Test://world")
            .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

    private IHostedService GetHeaderError(bool dual, string test)
    {
        var serviceCollection = GetServiceDescriptors();
        var errorComponent = Substitute.For<IErrorProcessor>();
        if (!dual)
        {
            Route.From($"Test://hello?TestName={test}")
                .AddHeader("One", () =>
                {
                    throw new NotImplementedException("whoops");
                })
                .To("Test://world")
                .Error(errorComponent)
                .Build(serviceCollection);
        }
        else
        {
            Route.From($"Test://hello?TestName={test}")
                .AddHeader("One", (x) =>
                {
                    throw new NotImplementedException("whoops");
                })
                .To("Test://world")
                .Error(errorComponent)
                .Build(serviceCollection);
        }
        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }

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