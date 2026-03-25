using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
using Xunit;
using NSubstitute;

namespace Kyameru.Component.Slack.Tests;

public class InflatorTests
{
    [Fact]
    public void ActivateToWorks()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "test" },
            { "MessageSource", "Body" }
        };
        var inflator = new Inflator();
        Assert.NotNull(inflator.CreateToComponent(headers, this.GetServiceProvider()));
    }

    [Fact]
    public void ActivateFromThrows()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "test" }
        };
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => { inflator.CreateFromComponent(headers, this.GetServiceProvider()); });
    }

    [Fact]
    public void RegisterFromThrows()
    {
        Assert.Throws<RouteNotAvailableException>(() => this.GetServiceDescriptors(true));
    }
    
    [Fact]
    public void RegisterScheduledThrowsException()
    {
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.RegisterScheduled(null));
    }
    
    [Fact]
    public void CreateScheduledThrowsException()
    {
        var inflator = new Inflator();
        Assert.Throws<NotImplementedException>(() => inflator.CreateScheduleComponent(null, null));
    }


    private IServiceCollection GetServiceDescriptors(bool tryFrom = false)
    {

        var serviceDescriptors = new ServiceCollection();
        serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Kyameru.Route>>();
        });

        var inflator = new Inflator();
        inflator.RegisterTo(serviceDescriptors);
        if (tryFrom)
        {
            inflator.RegisterFrom(serviceDescriptors);
        }

        return serviceDescriptors;

    }

    private IServiceProvider GetServiceProvider()
    {
        return this.GetServiceDescriptors().BuildServiceProvider();
    }



}