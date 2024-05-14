using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;
using Xunit;

namespace Kyameru.Tests.EntityTests;

public class RouteAttributesTests
{
    [Fact]
    public void UriParsesAsExpected()
    {
        var uri = "test://myhost/path?header1=value";
        var routeAttribute = new RouteAttributes(uri);
        var expected = new Dictionary<string, string>()
        {
            { "Host", "myhost" },
            { "Target", "/path" },
            { "header1", "value" }
        };

        Assert.Equal("Test", routeAttribute.ComponentName);
        Assert.All(routeAttribute.Headers, x =>
        {
            Assert.Equal(expected[x.Key], x.Value);
        });
    }

    [Theory]
    [InlineData("arn:partition:service:region:account-id:resource-id")]
    [InlineData("arn:partition:service:region:account-id:resource-type/resource-id")]
    [InlineData("arn:partition:service:region:account-id:resource-type:resource-id")]
    public void ArnParsesAsExpected(string arn)
    {
        var uri = $"test://{arn}?header1=value";
        var routeAttribute = new RouteAttributes(uri);
        var expected = new Dictionary<string, string>()
        {
            { "ARN", arn },
            { "header1", "value" }
        };
        Assert.Equal("Test", routeAttribute.ComponentName);
        Assert.All(routeAttribute.Headers, x =>
        {
            Assert.Equal(expected[x.Key], x.Value);
        });
    }

    [Fact]
    public void ArnWithNoHeadersParses()
    {
        var expected = "arn:partition:service:region:account-id:resource-id";
        var uri = $"test://{expected}";
        var routeAttribute = new RouteAttributes(uri);
        Assert.Single(routeAttribute.Headers);
        Assert.Equal(expected, routeAttribute.Headers["ARN"]);
    }
}
