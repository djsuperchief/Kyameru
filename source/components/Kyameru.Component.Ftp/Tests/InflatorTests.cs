using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Kyameru.Core.Exceptions;
using Xunit;
using NSubstitute;

namespace Kyameru.Component.Ftp.Tests;

public class InflatorTests
{
    [Fact]
    public void ActivateFromWorks()
    {
        IServiceProvider serviceProvider = this.GetServiceProvider();
        IFromChainLink fromComponent = new Ftp.Inflator().CreateFromComponent(this.GetHeaders(), serviceProvider);
        Assert.NotNull(fromComponent);
    }

    [Fact]
    public void ActivateToWorks()
    {
        IServiceProvider serviceProvider = this.GetServiceProvider();
        IToChainLink toComponent = new Ftp.Inflator().CreateToComponent(this.GetHeaders(), serviceProvider);
        Assert.NotNull(toComponent);
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

    private Dictionary<string, string> GetHeaders()
    {
        return new Dictionary<string, string>()
        {
            { "Host", "127.0.0.1" },
            { "Target", "Test" },
            { "Port", "21" },
            { "PollTime", "1" },
            { "Filter", "*" },
            { "UserName", "test" },
            { "Recursive", "true" },
            { "Delete", "false" },
        };
    }


    private IServiceCollection GetServiceDescriptors()
    {
        IServiceCollection serviceDescriptors = new ServiceCollection();
        serviceDescriptors.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return Substitute.For<ILogger<Route>>();
        });

        Inflator inflator = new Inflator();
        inflator.RegisterTo(serviceDescriptors);
        inflator.RegisterFrom(serviceDescriptors);

        return serviceDescriptors;
    }

    private IServiceProvider GetServiceProvider()
    {
        return this.GetServiceDescriptors().BuildServiceProvider();
    }
}