using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;
using Kyameru.TestUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ConfigTests;

public class LoadedConfigTests
{

    [Theory]
    [InlineData("JsonConfig.json", "MyComponent has processed Async")]
    [InlineData("JsonConfigPostProcessing.json", "MyPostComponent has processed Async")]
    public async Task CanLoadJsonConfigWithUrisAsync(string config, string expectedMessage)
    {
        var logger = Substitute.For<ILogger<Route>>();
        logger.IsEnabled(Arg.Is<LogLevel>(LogLevel.Information)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors(logger);
        var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
        Route.FromConfig(routeConfig, serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        AssertLogger(logger, expectedMessage);

    }

    [Theory]
    [InlineData("JsonConfigSchedule.json")]
    [InlineData("JsonConfigScheduleAt.json")]
    public async Task ScheduleHasExecuted(string config)
    {
        var expectedMessage = "Schedule Has Executed";
        var logger = Substitute.For<ILogger<Route>>();
        logger.IsEnabled(Arg.Is<LogLevel>(LogLevel.Debug)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors(logger);
        var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
        Route.FromConfig(routeConfig, serviceDescriptors);

        IServiceProvider provider = serviceDescriptors.BuildServiceProvider();
        IHostedService service = provider.GetService<IHostedService>();

        var thread = TestThread.CreateNew(service.StartAsync, 20);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        AssertLogger(logger, expectedMessage, LogLevel.Debug);

    }

    [Fact]
    public void BothScheduleEveryAndAtError()
    {
        var logger = Substitute.For<ILogger<Route>>();
        Assert.Throws<CoreException>(() =>
        {
            var config = "JsonConfigScheduleError.json";
            var serviceDescriptors = GetServiceDescriptors(logger);
            var routeConfig = RouteConfig.Load($"ConfigTests/{config}");
            Route.FromConfig(routeConfig, serviceDescriptors);
        });
    }

    [Fact]
    public void ServiceProviderSetupFromConfigurationWorks()
    {
        var logger = Substitute.For<ILogger<Route>>();
        var serviceDescriptors = GetServiceDescriptors(logger);
        var config = GetConfig();

        serviceDescriptors.Kyameru().FromConfiguration(config);
    }

    [Theory]
    [InlineData("ConfigTests/JsonConfigWhenBasic.json")]
    [InlineData("ConfigTests/JsonConfigWhenPost.json")]
    public async Task WhenExecutesCorrectly(string fileName)
    {
        var logger = Substitute.For<ILogger<Route>>();
        logger.IsEnabled(Arg.Is<LogLevel>(LogLevel.Information)).Returns(true);
        var serviceDescriptors = GetServiceDescriptors(logger);
        var routeConfig = RouteConfig.Load(fileName);
        Route.FromConfig(routeConfig, serviceDescriptors);


        var provider = serviceDescriptors.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();

        var thread = TestThread.CreateNew(service.StartAsync, 5);
        thread.Start();
        thread.WaitForExecution();
        await thread.CancelAsync();

        AssertLogger(logger, "ConfigWhenExecutes_To", LogLevel.Information);
    }

    private CancellationTokenSource GetCancellationToken(int timeInSeconds)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(timeInSeconds));
    }

    private IServiceCollection GetServiceDescriptors(ILogger<Route> logger)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient(sp => logger);
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }

    private Microsoft.Extensions.Configuration.IConfiguration GetConfig()
    {
        return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
    }

    private void AssertLogger(ILogger logger, string message, LogLevel logLevel = LogLevel.Information)
    {
        logger.Received(1).Log
        (
            logLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(x => x.ToString().Contains(message)),
            null,
            Arg.Any<Func<object, Exception, string>>()
        );
    }

}