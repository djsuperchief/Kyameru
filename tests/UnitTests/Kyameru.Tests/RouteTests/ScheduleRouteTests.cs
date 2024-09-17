using System;
using System.Collections;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core.Exceptions;
using Kyameru.Core.Utils;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.RouteTests;

public class ScheduleRouteTests
{
    [Fact]
    public void ScheduleIsSetupCorrectly()
    {
        var processingComponent = Substitute.For<IComponent>();
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test");
        builder.Schedule("* * * * *");
        Assert.Equal(1, builder.ToComponentCount);
        Assert.True(builder.IsScheduled);
    }

    [Fact]
    public void InvalidCronThrowsCorrectError()
    {
        var processingComponent = Substitute.For<IComponent>();
        var routeBuilder = Route.From("test://test");
        var builder = routeBuilder.To("test://test");
        Assert.Throws<CoreException>(() => builder.Schedule("cron"));
    }

}
