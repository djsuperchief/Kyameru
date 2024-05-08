using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Sns.Tests;

public class InjectionTests
{
    [Fact]
    public void RegisterFromThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void RegisterAtomicThrowsException()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterFrom(serviceCollection));
    }

    [Fact]
    public void RegisterToSucceeds()
    {
        var serviceCollection = new ServiceCollection();
        var inflator = new Inflator();
        inflator.RegisterTo(serviceCollection);
        Assert.True(serviceCollection.Contains(typeof(ITo), typeof(SnsTo)));
    }
}
