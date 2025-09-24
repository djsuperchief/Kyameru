using System.Net.Http.Json;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests;

public class ToTests : BaseTestWithMockHandler
{
    [Theory]
    [MemberData(nameof(MethodTests))]
    public void UrlIsValidForMethod(string method, bool isValid)
    {
        var inflator = new To();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders(method);
        var exception = Record.Exception(() => inflator.CreateToComponent(headers, serviceProvider));
        if (isValid)
        {
            Assert.Null(exception);
            var to = inflator.CreateToComponent(headers, serviceProvider);
            Assert.Equal(new HttpMethod(method), ((IRestTo)to).HttpMethod);
            Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestTo)to).Url);
        }
        else
        {
            Assert.NotNull(exception);
            Assert.Equal($"Error, method '{method}' is not a valid Http method available in this component.", exception.Message);
        }
    }

    [Theory]
    [InlineData("endpoint", false)]
    [InlineData("method", true)]
    [InlineData("Target", false)]
    [InlineData("Host", false)]
    public void UrlIsValidForRemovedRequiredHeaders(string header, bool isValid)
    {
        var inflator = new To();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Remove(header);
        var exception = Record.Exception(() => inflator.CreateToComponent(headers, serviceProvider));
        if (isValid)
        {
            Assert.Null(exception);
            var to = inflator.CreateToComponent(headers, serviceProvider);
            Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestTo)to).Url);
        }
        else
        {
            Assert.NotNull(exception);
            Assert.Equal($"Error, missing header '{header}'.", exception.Message);
        }
    }
    
    [Fact]
    public void CreateToCreatesChain()
    {
        var inflator = new To();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        var toChain = inflator.CreateToComponent(headers, serviceProvider);
        Assert.NotNull(toChain);
        Assert.Equal(HttpMethod.Get, ((IRestTo)toChain).HttpMethod);
        Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestTo)toChain).Url);
    }

    [Fact]
    public void QueryParametersAreCorrect()
    {
        var inflator = new To();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterTo(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Add("id", "20");
        headers.Add("date", "2025-01-01");
        var toChain = inflator.CreateToComponent(headers, serviceProvider);
        Assert.NotNull(toChain);
        Assert.Equal(HttpMethod.Get, ((IRestTo)toChain).HttpMethod);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=20&date=2025-01-01", ((IRestTo)toChain).Url);
    }

    [Fact]
    public async Task ToExecutesGetRequest_NoAuth()
    {
        var httpMessageHandlerMock = GetMockMessageHandler();
        var routeAttr = new RouteAttributes("rest://api/v1/hello?endpoint=localhost:8080");

        var to = new RestTo(httpMessageHandlerMock);
        to.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        await to.ProcessAsync(routable, default);

        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;

        Assert.Equal("GET", response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello", response.Url);
    }
    
    private IServiceCollection GetServiceCollection()
    {
        var collection = new ServiceCollection();
        // Any generic items here
        return collection;
    }

    private Dictionary<string, string> GetValidHeaders(string method = "get") => new Dictionary<string, string>()
    {
        { "endpoint", "localhost:8080" },
        { "Host", "api" },
        { "Target", "/v1/hello" },
        { "method", method }
    };

    public static IEnumerable<object[]> MethodTests()
    {
        var methods = new Dictionary<string, bool>()
        {
            { "post", true },
            { "put", true },
            { "get", true },
            { "delete", true },
            { "connect", true },
            { "head", true },
            { "options", true },
            { "trace", true },
            { "patch", true },
            { "invalid", false }
        };
        
        foreach(var method in methods)
        {
            yield return [method.Key, method.Value];
        }
    }
}