using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
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
        Assert.NotNull(inflator.CreateFromComponent(headers, false, serviceProvider));
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
    public void AtomicThrows()
    {
        var inflator = new Inflator();
        Assert.Throws<RouteNotAvailableException>(() => inflator.CreateAtomicComponent(null));
    }
}