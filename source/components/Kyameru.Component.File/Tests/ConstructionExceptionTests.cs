using Xunit;
using System;
using System.Collections.Generic;

namespace Kyameru.Component.File.Tests;

public class ConstructionExceptionTests
{
    private readonly IServiceProvider serviceProvider;
    private ServiceHelper serviceHelper = new ServiceHelper();

    public ConstructionExceptionTests()
    {
        serviceProvider = serviceHelper.GetServiceProvider();
    }

    [Fact]
    public void EmptyTargetThrowsError()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Notifications", "Changed" },
        };
        var inflator = new Inflator();

        Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, serviceProvider));
    }

    [Fact]
    public void EmptyNotificationsThrowsError()
    {
        var headers = new Dictionary<string, string>()
        {
            { "Target", "C:/test" },
        };
        var inflator = new Inflator();

        Assert.Throws<ArgumentException>(() => _ = inflator.CreateFromComponent(headers, serviceProvider));
    }
}