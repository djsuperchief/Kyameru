using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Tests.Mocks;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
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
    public void ToPostProcessingRegistersReflection_RouteBuilder()
    {
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test", "MyComponent");
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

    [Fact]
    public void ToPostProcessingRegistersReflection_Builder()
    {
        var builder = GetBuilder();
        var final = builder.To("test://test", "MyComponent");
        Assert.Equal(2, builder.ToComponentCount);
    }

    [Fact]
    public async Task ToPostProcessConcreteExecutesAsExpected()
    {
        var mockProcessor = Substitute.For<IProcessComponent>();
        Routable result = null;
        mockProcessor.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("PostProcessing", "true");
            result = x.Arg<Routable>();
            return Task.CompletedTask;
        });
        var services = GetServiceDescriptors();
        Kyameru.Route.From("injectiontest:///test")
            .To("injectiontest:///somewhere", mockProcessor)
            .Build(services);
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("true", result.Headers.TryGetValue("PostProcessing", string.Empty));
    }

    [Fact]
    public async Task ToPostProcessDiExecutesAsExpected()
    {
        var mockProcessor = Substitute.For<IMyComponent>();
        Routable result = null;
        mockProcessor.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
        {
            x.Arg<Routable>().SetHeader("PostProcessing", "true");
            result = x.Arg<Routable>();
            return Task.CompletedTask;
        });
        var services = GetServiceDescriptors();
        services.AddTransient<IMyComponent>(x =>
        {
            return mockProcessor;
        });
        Kyameru.Route.From("injectiontest:///test")
            .To<IMyComponent>("injectiontest:///somewhere")
            .Build(services);
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("true", result.Headers.TryGetValue("PostProcessing", string.Empty));
    }

    [Fact]
    public async Task ToPostProcessActionExecutesAsExpected()
    {
        Routable result = null;
        var services = GetServiceDescriptors();
        Kyameru.Route.From("injectiontest:///test")
            .To("injectiontest:///somewhere", (Routable x) =>
            {
                x.SetHeader("PostProcessing", "true");
                result = x;
            })
            .Build(services);
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("true", result.Headers.TryGetValue("PostProcessing", string.Empty));
    }

    [Fact]
    public async Task ToPostProcessFuncExecutesAsExpected()
    {
        Routable result = null;
        var services = GetServiceDescriptors();
        Kyameru.Route.From("injectiontest:///test")
            .To("injectiontest:///somewhere", async (Routable x) =>
            {
                x.SetHeader("PostProcessing", "true");
                result = x;
                await Task.CompletedTask;
            })
            .Build(services);
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("true", result.Headers.TryGetValue("PostProcessing", string.Empty));
    }

    [Fact]
    public async Task ToPostProcessReflectionExecutesAsExpected()
    {
        var mockProcessor = Substitute.For<IMyComponent>();
        Routable result = null;
        mockProcessor.ProcessAsync(default, default).ReturnsForAnyArgs(x =>
       {
           x.Arg<Routable>().SetHeader("PostProcessing", "true");
           result = x.Arg<Routable>();
           return Task.CompletedTask;
       });
        var services = GetServiceDescriptors();
        services.AddTransient<IMyComponent>(x => mockProcessor);
        Kyameru.Route.From("injectiontest:///test")
            .To("injectiontest:///plop", "Mocks.MyComponent")
            .To<IMyComponent>("injectiontest:///somewhere")
            .Build(services);
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 3);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.NotNull(result);
        Assert.Equal("Yes", result.Headers.TryGetValue("ComponentRan", string.Empty));
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var logger = Substitute.For<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger;
        });
        serviceCollection.AddTransient<Mocks.IMyComponent, Mocks.MyComponent>();

        return serviceCollection;
    }

    private Core.Builder GetBuilder()
    {
        var processingComponent = new MyComponent();
        var routeBuilder = Route.From("test://test");
        return routeBuilder.To("test://test", processingComponent);
    }
}
