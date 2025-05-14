using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ChainTests;

public class ScheduleChainTests
{
    [Fact]
    public void CanBuildScheduledComponent()
    {
        var serviceCollection = GetServiceDescriptors();
        var exception = Record.Exception(() =>
        {
            Kyameru.Route.From("test://test")
                .To("test://test")
                .Id("ScheduleTest")
                .ScheduleEvery(Core.Enums.TimeUnit.Hour, 1)
                .Build(serviceCollection);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void EveryAndAtThrowException()
    {
        var serviceCollection = GetServiceDescriptors();
        Assert.Throws<CoreException>(() =>
        {
            Kyameru.Route.From("test://test")
            .To("test://test")
            .Id("ScheduledTest")
            .ScheduleAt(Core.Enums.TimeUnit.Hour, 1)
            .ScheduleEvery(Core.Enums.TimeUnit.Minute, 1)
            .Build(serviceCollection);
        });
    }

    [Fact]
    public async Task ScheduledComponentExecutes()
    {
        var serviceCollection = GetServiceDescriptors();
        var mockNext = Substitute.For<IProcessor>();
        var thread = TestThread.CreateDeferred();
        Routable output = null;
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Kyameru.Core.Utils.TimeProvider.Current = simulatedTime;
        Component.Generic.Builder.Create()
            .WithTo(x =>
            {
                thread.Continue();
            }).Build(serviceCollection);

        mockNext.ProcessAsync(Arg.Any<Routable>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            output = x.Arg<Routable>();
            return Task.CompletedTask;
        });
        Kyameru.Route.From("test://test")
        .Process(mockNext)
        .To("generic://test")
        .Id("scheduled_test")
        .ScheduleEvery(Core.Enums.TimeUnit.Minute, 1)
        .Build(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        testDate = testDate.AddMinutes(2);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        thread.WaitForExecution();

        // Cancel the thread, wait for exit
        await thread.CancelAsync();

        Assert.Equal("2", output.Headers["Counter"]);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var logger = Substitute.For<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger;
        });

        return serviceCollection;
    }

    private CancellationTokenSource GetCancellationToken(int timeInSeconds)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(timeInSeconds));
    }
}
