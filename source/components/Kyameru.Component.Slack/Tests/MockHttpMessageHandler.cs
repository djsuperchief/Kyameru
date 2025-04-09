using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Slack.Tests;

public class MockHttpMessageHandler(string content, HttpStatusCode statusCode) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        });
    }

    public static HttpMessageHandler Create(string content, HttpStatusCode statusCode)
    {
        return new MockHttpMessageHandler(content, statusCode);
    }
}
