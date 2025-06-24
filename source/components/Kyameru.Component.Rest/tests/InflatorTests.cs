using System;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest.Tests;

public class InflatorTests
{
    [Fact]
    public void CreateToCreatesChain()
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        var toChain = inflator.CreateToComponent(headers, serviceProvider);
        Assert.NotNull(toChain);
        Assert.Equal("get", ((IRestTo)toChain).HttpMethod);
        Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestTo)toChain).Url);
    }

    [Theory]
    [InlineData("get")]
    [InlineData("put")]
    [InlineData("post")]
    [InlineData("delete")]
    [InlineData("notvalid", false)]
    public void MethodIsValid(string method, bool isValid = true)
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Add("method", method);
        if (isValid)
        {
            var toChain = inflator.CreateToComponent(headers, serviceProvider);
            Assert.NotNull(toChain);
            Assert.Equal(method, ((IRestTo)toChain).HttpMethod);
            Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestTo)toChain).Url);
        }
        else
        {
            Assert.Throws<Core.Exceptions.ComponentException>(() => inflator.CreateToComponent(headers, serviceProvider));
        }
    }

    [Fact]
    public void QueryParametersAreCorrect()
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Add("id", "20");
        headers.Add("date", "2025-01-01");
        var toChain = inflator.CreateToComponent(headers, serviceProvider);
        Assert.NotNull(toChain);
        Assert.Equal("get", ((IRestTo)toChain).HttpMethod);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=20&date=2025-01-01", ((IRestTo)toChain).Url);
    }

    private IServiceCollection GetServiceCollection()
    {
        var collection = new ServiceCollection();
        // Any generic items here
        return collection;
    }

    private Dictionary<string, string> GetValidHeaders() => new Dictionary<string, string>()
        {
            { "endpoint", "localhost:8080" },
            { "Host", "api" },
            { "Target", "/v1/hello" }
        };

}
