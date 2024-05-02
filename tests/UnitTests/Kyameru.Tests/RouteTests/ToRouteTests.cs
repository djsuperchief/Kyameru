using System;
using Kyameru.Tests.Mocks;
using Xunit;

namespace Kyameru.Tests.RouteTests;

/// <summary>
/// Start of a more organised testing structure.
/// </summary>
public class ToRouteTests
{
    [Fact]
    public void ToPostProcessingRegisters()
    {
        var processingComponent = new MyComponent();
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test", processingComponent);
        Assert.Equal(1, builder.ToComponentCount);
    }
}
