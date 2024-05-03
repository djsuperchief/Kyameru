using System;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Tests.Mocks;
using Xunit;

namespace Kyameru.Tests.RouteTests;

/// <summary>
/// Start of a more organised testing structure.
/// </summary>
public class ToRouteTests
{
    [Fact]
    public void ToPostProcessingRegistersConcrete_RouteBuilder()
    {
        var processingComponent = new MyComponent();
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test", processingComponent);
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersDI_RouteBuilder()
    {
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To<IMyComponent>("test://test");
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersAction_RouteBuilder()
    {
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test", (Routable x) =>
        {
            x.SetHeader("Test", "Test");
        });
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersFunc_RouteBuilder()
    {
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test", async (Routable x) =>
        {
            x.SetHeader("Test", "Test");

            await Task.CompletedTask;
        });
        Assert.Equal(1, builder.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersConcrete_Builder()
    {
        var processingComponent = new MyComponent();
        var builder = GetBuilder();
        var final = builder.To("test://test", processingComponent);
        Assert.Equal(2, final.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersDI_Builder()
    {
        var processingComponent = new MyComponent();
        var builder = GetBuilder();
        var final = builder.To<IMyComponent>("test://test");
        Assert.Equal(2, final.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersAction_Builder()
    {
        var processingComponent = new MyComponent();
        var builder = GetBuilder();
        var final = builder.To("test://test", (Routable x) =>
        {
            x.SetHeader("Test", "Test");
        });
        Assert.Equal(2, final.ToComponentCount);
    }

    [Fact]
    public void ToPostProcessingRegistersFunc_Builder()
    {
        var processingComponent = new MyComponent();
        var builder = GetBuilder();
        var final = builder.To("test://test", async (Routable x) =>
        {
            x.SetHeader("Test", "Test");

            await Task.CompletedTask;
        });
        Assert.Equal(2, final.ToComponentCount);
    }

    private Core.Builder GetBuilder()
    {
        var processingComponent = new MyComponent();
        var routeBuilder = Route.From("test://test");
        return routeBuilder.To("test://test", processingComponent);
    }
}
