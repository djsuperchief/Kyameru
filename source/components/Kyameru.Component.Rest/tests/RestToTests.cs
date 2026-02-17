using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Extensions;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Kyameru.Component.Rest.Tests;

public class RestToTests : BaseTestWithMockHandler
{
    [Theory]
    [MemberData(nameof(MethodTests))]
    public void UrlIsValidForMethod(string method, bool isValid)
    {
        var inflator = new EventInflator();
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
    [InlineData("method", true)]
    [InlineData("Target", false)]
    [InlineData("Host", false)]
    public void UrlIsValidForRemovedRequiredHeaders(string header, bool isValid)
    {
        var inflator = new EventInflator();
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
        var inflator = new EventInflator();
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
        var inflator = new EventInflator();
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

    [Theory]
    [MemberData(nameof(JustMethodTests))]
    public async Task MethodExecutesGetRequest_NoAuth(string method)
    {
        var httpMessageHandlerMock = GetMockMessageHandler();
        var routeAttr = new RouteAttributes($"rest://localhost:8080/api/v1/hello?method={method}");
        var to = new RestTo(new HttpContentFactory(), httpMessageHandlerMock);
        to.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        await to.ProcessAsync(routable, CancellationToken.None);

        var response = (routable.Body as JsonContent).Value as Entities.GetResponse;

        Assert.Equal(method.ToUpper(), response.Method);
        Assert.Equal("https://localhost:8080/api/v1/hello", response.Url);
    }

    [Theory]
    [MemberData(nameof(HttpBodyTests))]
    public async Task HttpPostBodyIsCorrect(string contentType, object input, object output, string method, Func<Routable, Task<object>> getResponse)
    {
        var httpMessageHandlerMock = GetMockMessageHandler();
        var routeAttr = new RouteAttributes($"rest://localhost:8080/api/v1/hello?method={method}");
        var to = new RestTo(new HttpContentFactory(), httpMessageHandlerMock);
        to.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        routable.SetBody(input);
        routable.SetHeader("HttpContentType", contentType);
        await to.ProcessAsync(routable, CancellationToken.None);

        var toCompare = await getResponse(routable);
        
        Assert.Equivalent(output, toCompare);
    }
    
    private IServiceCollection GetServiceCollection()
    {
        var collection = new ServiceCollection();
        // Any generic items here
        return collection;
    }

    private Dictionary<string, string> GetValidHeaders(string method = "get") => new Dictionary<string, string>()
    {
        { "Host", "localhost" },
        { "Target", "/api/v1/hello" },
        { "method", method },
        { "Port", "8080"}
    };
}