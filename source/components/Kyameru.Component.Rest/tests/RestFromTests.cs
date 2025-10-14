using System.Net.Http.Json;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Component.Rest.Implementation;
using Kyameru.Component.Rest.Messages;
using Kyameru.Component.Rest.Tests.Utils;
using Kyameru.Core.Comms;
using Kyameru.Core.Entities;
using Kyameru.Core.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Rest.Tests;

public class RestFromTests : BaseTestWithMockHandler
{
    [Theory]
    [MemberData(nameof(MethodTests))]
    public void UrlIsValidForMethod(string method, bool isValid)
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterFrom(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders(method);
        var exception = Record.Exception(() => inflator.CreateFromEvent(headers, serviceProvider));
        if (isValid)
        {
            Assert.Null(exception);
            var from = inflator.CreateFromEvent(headers, serviceProvider);
            Assert.Equal(new HttpMethod(method), ((IRestFrom)from).HttpMethod);
            Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestFrom)from).Url);
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
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterFrom(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Remove(header);
        var exception = Record.Exception(() => inflator.CreateFromEvent(headers, serviceProvider));
        if (isValid)
        {
            Assert.Null(exception);
            var from = inflator.CreateFromEvent(headers, serviceProvider);
            Assert.Equal("https://localhost:8080/api/v1/hello", ((IRestFrom)from).Url);
        }
        else
        {
            Assert.NotNull(exception);
            Assert.Equal($"Error, missing header '{header}'.", exception.Message);
        }
    }
    
    [Fact]
    public void QueryParametersAreCorrect()
    {
        var inflator = new Inflator();
        var serviceCollection = GetServiceCollection();
        inflator.RegisterFrom(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var headers = GetValidHeaders();
        headers.Add("id", "20");
        headers.Add("date", "2025-01-01");
        var fromChain = inflator.CreateFromEvent(headers, serviceProvider);
        Assert.NotNull(fromChain);
        Assert.Equal(HttpMethod.Get, ((IRestFrom)fromChain).HttpMethod);
        Assert.Equal("https://localhost:8080/api/v1/hello?id=20&date=2025-01-01", ((IRestFrom)fromChain).Url);
    }
    
    [Theory]
    [MemberData(nameof(JustMethodTests))]
    public async Task MethodExecutesGetRequest_NoAuth(string method)
    {
        var httpMessageHandlerMock = GetMockMessageHandler();
        var routeAttr = new RouteAttributes($"rest://localhost:8080/api/v1/hello?method={method}");
        var from = new RestFrom(new HttpContentFactory(), httpMessageHandlerMock);
        from.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        var message = CommsMessage.Create<HttpMessageData>("Test", new HttpMessageData()
        {
            Headers = new Dictionary<string, string>()
        });
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            routable = eventData.Data;
            await Task.CompletedTask;
        };
        
        await from.ProcessAsync(message, CancellationToken.None);

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
        var from = new RestFrom(new HttpContentFactory(), httpMessageHandlerMock);
        from.SetHeaders(routeAttr.Headers);
        var routable = new Routable(new Dictionary<string, string>(), "test");
        Dictionary<string, string> messageHeaders = new Dictionary<string, string>();
        messageHeaders.Add("HttpContentType", contentType);
        var message = CommsMessage.Create<HttpMessageData>("Test", new HttpMessageData()
        {
            Headers = messageHeaders,
            Data = input
        });
        
        from.OnActionAsync += async (object sender, RoutableEventData eventData) =>
        {
            routable = eventData.Data;
            await Task.CompletedTask;
        };
        
        await from.ProcessAsync(message, CancellationToken.None);

        var toCompare = await getResponse(routable);
        
        Assert.Equivalent(output, toCompare);
    }
    
    private Dictionary<string, string> GetValidHeaders(string method = "get") => new Dictionary<string, string>()
    {
        { "Host", "localhost" },
        { "Target", "/api/v1/hello" },
        { "method", method },
        { "Port", "8080"}
    };
    
    private IServiceCollection GetServiceCollection()
    {
        var collection = new ServiceCollection();
        // Any generic items here
        return collection;
    }
}