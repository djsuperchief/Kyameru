using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.Tests.Mocks;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
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
        var mockNext = new Mock<IProcessComponent>();
        Routable output = null;
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Kyameru.Core.Utils.TimeProvider.Current = simulatedTime;
        var cancellationTokenSource = GetCancellationToken(120);

        // TODO: This needs to be a proper Kyameru setup (like the other route tests)
        mockNext.Setup(x => x.ProcessAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken y) =>
        {
            output = x;
            await Task.CompletedTask;
        });
        Kyameru.Route.From("test://test")
        .Process(mockNext.Object)
        .To("test://test")
        .Id("scheduled_test")
        .ScheduleEvery(Core.Enums.TimeUnit.Minute, 1)
        .Build(serviceCollection);
        IServiceProvider provider = serviceCollection.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 10);
        thread.Start();
        thread.WaitForExecution();
        testDate = testDate.AddMinutes(2);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        thread.WaitForExecution();

        // Cancel the thread, wait for exit
        await thread.Cancel();

        Assert.Equal("2", output.Headers["Counter"]);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var logger = new Mock<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger.Object;
        });

        return serviceCollection;
    }

    private CancellationTokenSource GetCancellationToken(int timeInSeconds)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(timeInSeconds));
    }
}
