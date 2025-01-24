using System;
using Kyameru.Core.Entities;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

/// <summary>
/// Conditional route tests.
/// </summary>
public class ConditionalTests
{
    [Fact]
    public void WhenRegistersToComponent()
    {
        var builder = Kyameru.Route.From("test://test")
        .When((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void RouteAttributesRegistersCondition()
    {
        var attribute = new RouteAttributes((Routable x) => x.Body.ToString() == "Test", "testto://test");
        Assert.True(attribute.HasCondition);
    }
}
