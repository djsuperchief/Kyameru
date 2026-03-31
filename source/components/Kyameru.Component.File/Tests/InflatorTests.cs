using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kyameru.Component.File.Tests;

public class InflatorTests
{
    private IServiceProvider serviceProvider;
    private ServiceHelper serviceHelper = new ServiceHelper();

    public InflatorTests()
    {
        serviceProvider = serviceHelper.GetServiceProvider();
    }

    [Fact]
    public void CanGetFrom()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "test/" },
            { "Notifications", "Created" },
            { "Filter", "*.tdd" },
            { "SubDirectories", "true" }
        };
        var inflator = new Inflator();
        Assert.NotNull(inflator.CreateFromComponent(headers, serviceProvider));
    }

    [Fact]
    public void CanGetTo()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "test/target" },
            { "Action", "Move" },
            { "Overwrite","true" }
        };

        var inflator = new Inflator();
        Assert.NotNull(inflator.CreateToComponent(headers, serviceProvider));
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

    [Fact]
    public void RegisterDependenciesDoesNothing()
    {
        var mockServices = Substitute.For<IServiceCollection>();
        var inflator = new Inflator();
        inflator.RegisterDependencies(mockServices, default, default);
        Assert.Empty(mockServices.ReceivedCalls());
    }
}