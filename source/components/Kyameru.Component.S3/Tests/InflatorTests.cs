using Microsoft.Extensions.DependencyInjection;


namespace Kyameru.Component.S3.Tests;

public class InflatorTests
{
    [Fact]
    public void RegisterFromThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void CreateFromThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.CreateFromComponent(new Dictionary<string, string>(), false, provider));
    }

    [Fact]
    public void CreateAtomicThrowsNotImplemented()
    {
        var serviceCollection = new ServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.CreateAtomicComponent(new Dictionary<string, string>()));
    }

    [Fact]
    public void RegisterToComponentSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(S3To)));
    }
}
