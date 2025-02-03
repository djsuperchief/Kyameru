using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kyameru.Tests.ConfigTests;

public class LoadedConfigTests
{
    private readonly Mock<ILogger<Route>> logger = new Mock<ILogger<Route>>();

    [Theory]
    [InlineData("JsonConfig.json", "MyComponent has processed Async")]
    [InlineData("JsonConfigPostProcessing.json", "MyPostComponent has processed Async")]
    [InlineData("JsonConfigSchedule.json", "MyComponent has processed Async")]
    public async Task CanLoadJsonConfigWithUrisAsync(string config, string expectedMessage)
    {
        var hasLogged = false;
        logger.Reset();
        logger.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
            .Callback(new InvocationAction(invocation =>
            {
                var logLevel =
                    (LogLevel)invocation
                        .Arguments[0]; // The first two will always be whatever is specified in the setup above
                var eventId =
                    (EventId)invocation.Arguments[1]; // so I'm not sure you would ever want to actually use them
                var state = invocation.Arguments[2];
                var exception = (Exception)invocation.Arguments[3];
                var formatter = invocation.Arguments[4];

                var invokeMethod = formatter.GetType().GetMethod("Invoke");
                var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                if (!hasLogged)
                {
                    hasLogged = logMessage.Contains(expectedMessage);
                }
            }));
        logger.Setup(x => x.IsEnabled(LogLevel.Information)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors();
        var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
        Route.FromConfig(routeConfig, serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.True(hasLogged);

    }

    [Fact]
    public async Task ScheduleHasExecuted()
    {
        var config = "JsonConfigSchedule.json";
        var expectedMessage = "Scheduled chain executing...";
        var hasLogged = false;
        logger.Reset();
        logger.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
            .Callback(new InvocationAction(invocation =>
            {
                var logLevel =
                    (LogLevel)invocation
                        .Arguments[0]; // The first two will always be whatever is specified in the setup above
                var eventId =
                    (EventId)invocation.Arguments[1]; // so I'm not sure you would ever want to actually use them
                var state = invocation.Arguments[2];
                var exception = (Exception)invocation.Arguments[3];
                var formatter = invocation.Arguments[4];

                var invokeMethod = formatter.GetType().GetMethod("Invoke");
                var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                if (!hasLogged)
                {
                    hasLogged = logMessage.Contains(expectedMessage);
                }
            }));
        logger.Setup(x => x.IsEnabled(LogLevel.Information)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors();
        var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
        Route.FromConfig(routeConfig, serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.True(hasLogged);

    }

    private IServiceCollection GetServiceDescriptors()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(sp => this.logger.Object);
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }

}