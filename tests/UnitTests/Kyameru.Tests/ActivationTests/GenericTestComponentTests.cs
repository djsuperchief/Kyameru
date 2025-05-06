using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Error;
using Kyameru.Component.Generic;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Kyameru.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.ActivationTests;

/// <summary>
/// This test class is so that we can use a generic component in the test project
/// and get rid of the horrific ones we have now. Where possible, we should use
/// this to make it easier to build custom flows in tests.
/// </summary>
public class GenericTestComponentTests
{
    [Fact]
    public async Task NonAtomic()
    {
        var routable = new Routable(new Dictionary<string, string>(), null);
        var thread = TestThread.CreateDeferred(20);
        var services = GetServiceDescriptors();

        Component.Generic.Builder.Create()
            .WithFrom(() => new Routable(new Dictionary<string, string>(), "Test"))
            .WithTo((Routable x) =>
            {
                x.SetBody<string>("To Done");
                routable = x;
                thread.Continue();
            })
            .Build(services);


        Route.From("generic:///test")
        .To("generic:///test")
        .Id("dynamic-component")
        .Build(services);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IHostedService>();
        thread.SetThread(service.StartAsync);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.Equal("To Done", routable.Body);
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });


        return serviceCollection;
    }


}
