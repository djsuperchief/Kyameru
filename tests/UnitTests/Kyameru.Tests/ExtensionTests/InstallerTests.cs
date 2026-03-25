using Kyameru.Core.Enums;
using Kyameru.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kyameru.Tests.ExtensionTests;

public class InstallerTests
{
    [Fact]
    public static void CanRegisterKyameruDependency()
    {
        var serviceDescriptors = new ServiceCollection();
        var identity = serviceDescriptors.RegisterKyameruDependency<IMyComponent>(ChainLinkDependencyType.From, () =>  new MyComponent());
        var services = serviceDescriptors.BuildServiceProvider();
        
        var requested = services.GetKeyedService<IMyComponent>(identity.Id);
        Assert.NotNull(requested);
    }
}