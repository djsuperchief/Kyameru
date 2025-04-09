using Kyameru.Core.Enums;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

public class ActivationExceptionTests
{
    private readonly ILogger<Route> _logger = Substitute.For<ILogger<Route>>();

    [Theory]
    [InlineData("Invalidfromcomponent", "invalidFromComponent", "test", "test")]
    [InlineData("Atomic", "test", "invalid", "test")]
    [InlineData("Invalidto", "test", "test", "InvalidTo")]
    public void ComponentInvalid(string expected, string from, string atomic, string to)
    {
        var errorComponent = string.Empty;
        try
        {
            _ = this.GetHostedService(from, atomic, to);
        }
        catch (ActivationException ex)
        {
            errorComponent = ex.Component;
        }
        Assert.Equal(expected, errorComponent);
    }

    [Fact]
    public void ScheduleComponentThrowsErrorOnRegister()
    {
        var exception = Record.Exception(() => this.GetHostedService("injectiontest", "test", "test", TimeUnit.Minute));
        Assert.NotNull(exception);
        Assert.IsType<ActivationException>(exception);
        Assert.Equal("Component 'Injectiontest' does not support scheduling", exception.Message);
    }

    [Fact]
    public void ScheduleComponentThrowsErrorOnBuild()
    {
        var exception = Record.Exception(() => this.GetHostedService("error", "test", "test", TimeUnit.Minute));
        Assert.NotNull(exception);
        Assert.IsType<ActivationException>(exception);
        Assert.Equal("Component 'Error' does not support scheduling", exception.Message);
    }

    private IHostedService GetHostedService(
            string fromHost,
            string atomicHost,
            string toHost,
            TimeUnit? schedule = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return _logger;
        });
        var from = $"{fromHost}://hello";
        var to = $"{toHost}://hello";
        var atomic = $"{atomicHost}://hello";

        var builder = Kyameru.Route.From(from)
            .To(to)
            .Atomic(atomic);
        if (schedule != null)
        {
            builder.ScheduleEvery(schedule.Value);
        }
        builder.Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetService<IHostedService>();
    }
}