using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.Configuration;
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

    [Theory]
    [InlineData("JsonConfigSchedule.json")]
    [InlineData("JsonConfigScheduleAt.json")]
    public async Task ScheduleHasExecuted(string config)
    {
        var expectedMessage = "Schedule Has Executed";
        var hasLogged = false;
        logger.Reset();
        logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
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
        logger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors();
        var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
        Route.FromConfig(routeConfig, serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();

        var thread = TestThread.CreateNew(service.StartAsync, 20);
        thread.Start();
        thread.WaitForExecution();
        await thread.Cancel();

        Assert.True(hasLogged);

    }

    [Fact]
    public void BothScheduleEveryAndAtError()
    {
        Assert.Throws<CoreException>(() =>
        {
            var config = "JsonConfigScheduleError.json";
            var serviceDescriptors = GetServiceDescriptors();
            var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
            Route.FromConfig(routeConfig, serviceDescriptors);
        });
    }

    [Fact]
    public void ServiceProviderSetupFromConfigurationWorks()
    {
        var serviceDescriptors = GetServiceDescriptors();
        var config = GetConfig();

        serviceDescriptors.Kyameru().FromConfiguration(config);
    }

    [Theory]
    [InlineData("ConfigTests/JsonConfigWhenBasic.json")]
    [InlineData("ConfigTests/JsonConfigWhenPost.json")]
    public async Task WhenExecutesCorrectly(string fileName)
    {
        var serviceDescriptors = GetServiceDescriptors();
        var routeConfig = RouteConfig.Load(fileName);
        Route.FromConfig(routeConfig, serviceDescriptors);
        logger.Reset();
        logger.Setup(x => x.IsEnabled(LogLevel.Information)).Returns(true);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();

        var thread = TestThread.CreateNew(service.StartAsync, 5);
        thread.Start();
        thread.WaitForExecution();
        await thread.Cancel();

        AssertLogger("ConfigWhenExecutes_To", LogLevel.Information);
    }

    private CancellationTokenSource GetCancellationToken(int timeInSeconds)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(timeInSeconds));
    }

    private IServiceCollection GetServiceDescriptors()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(sp => this.logger.Object);
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }

    private Microsoft.Extensions.Configuration.IConfiguration GetConfig()
    {
        return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
    }

    private void AssertLogger(string message, LogLevel logLevel = LogLevel.Information)
    {
        logger.Verify(x => x.Log(
            logLevel,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((logMessage, type) => logMessage.ToString().Contains(message)),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

}