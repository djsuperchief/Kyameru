using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Chain;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task ScheduledComponentExecutes()
    {
        var mockNext = new Mock<IChain<Routable>>();
        Routable output = null;
        var waitTimer = new AutoResetEvent(false);
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Kyameru.Core.Utils.TimeProvider.Current = simulatedTime;
        var cancellationToken = GetCancellationToken(120);

        // TODO: This needs to be a proper Kyameru setup (like the other route tests)
        // mockNext.Setup(x => x.HandleAsync(It.IsAny<Routable>(), It.IsAny<CancellationToken>())).Callback(async (Routable x, CancellationToken y) =>
        // {
        //     output = x;
        //     await Task.CompletedTask;
        // });


        // var scheduled = new ScheduleChainFacade(new MockScheduled(), mockNext.Object, new Mock<ILogger>().Object, "test", false, false, new Core.Entities.Schedule(Core.Enums.TimeUnit.Minute, 1, true));
        // var testThread = new Thread(async () => {
        //     await scheduled.Run(cancellationToken);
        // });

        // waitTimer.WaitOne(TimeSpan.FromSeconds(10));
        // testDate = testDate.AddMinutes(2);
        // simulatedTime.UtcNow.Returns(testDate);
        // simulatedTime.Now.Returns(testDate.ToLocalTime());
        // waitTimer.WaitOne(TimeSpan.FromSeconds(10));


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

    private CancellationToken GetCancellationToken(int timeInSeconds)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(timeInSeconds)).Token;
    }
}
