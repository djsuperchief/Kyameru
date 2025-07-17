using System;
using Microsoft.Extensions.DependencyInjection;
using Kyameru.Core;
using Kyameru.Component.Rest.Tests.Utils;
using NSubstitute;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Kyameru.Component.Rest.Tests.Entities;

namespace Kyameru.Component.Rest.Tests;

public class BuilderExtensionsTests
{
    [Fact]
    public async Task CanAddAPITokenToFrom_NoHeader()
    {
        var httpMessageHandlerMock = GetMockHttpMessageHandler();
        var serviceCollection = GetServiceDescriptors();
        var messageExtractor = Substitute.For<IMessageExtractor>();
        var routable = new Routable(new Dictionary<string, string>(), "test");

        messageExtractor.ProcessAsync(default, default).Returns(x =>
        {
            routable = x.Arg<Routable>();
            return Task.CompletedTask;
        });

        Route.From("rest://api/v1/hello?endpoint=localhost:8080")
            .AuthWithApiToken("mytoken")
            .To("rest://api/v1/hello?endpoint=localhost:8080")
            .Build(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetRequiredService<IHostedService>();
        var thread = TestThread.CreateNew(service.StartAsync, 5);
        thread.StartAndWait();
        await thread.CancelAsync();

        Assert.NotNull(routable);
        Assert.Equal("Bearer:mytoken", routable.Headers["Auth"]);

    }

    private static MockHttpMessageHandler GetMockHttpMessageHandler()
    {
        var httpMessageHandlerMock = Substitute.ForPartsOf<MockHttpMessageHandler>();
        httpMessageHandlerMock.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
        .ReturnsForAnyArgs(x =>
        {
            var requestMessage = x.Arg<HttpRequestMessage>();

            var response = new HttpResponseMessage()
            {
                Content = JsonContent.Create<Entities.GetResponse>(new Entities.GetResponse()
                {
                    Method = requestMessage.Method.ToString(),
                    Url = requestMessage.RequestUri!.ToString(),
                })
            };

            if (requestMessage.Headers.Contains("Auth"))
            {
                response.Headers.Add("Auth", requestMessage.Headers.Authorization!.ToString());
            }

            return response;
        });

        return httpMessageHandlerMock;
    }

    private IServiceCollection GetServiceDescriptors()
    {
        var logger = Substitute.For<ILogger<Route>>();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILogger<Kyameru.Route>>(sp =>
        {
            return logger;
        });

        return serviceCollection;
    }
}
