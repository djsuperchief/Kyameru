using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.RabbitMQ.Tests;

public class InflatorTests
{
    [Fact]
    public void CreateScheduledThrowsNotSupportedException()
    {
        var serviceCollection = new ServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        var ex = Assert.Throws<NotSupportedException>(() => inflator.CreateScheduleComponent(new Dictionary<string, string>(), provider));
        Assert.Equal("RabbitMQ component does not support scheduling", ex.Message);
    }

    [Fact]
    public void RegisterScheduledThrowsNotSupportedException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        var ex = Assert.Throws<NotSupportedException>(() => inflator.RegisterScheduled(serviceCollection));
        Assert.Equal("RabbitMQ component does not support scheduling", ex.Message);
    }
}