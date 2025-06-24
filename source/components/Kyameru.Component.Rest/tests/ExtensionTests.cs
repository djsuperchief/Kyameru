using System;
using Kyameru.Component.Rest.Extensions;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Tests
{
    public class ExtensionTests
    {
        [Theory]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080", true)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&https=true", true)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&https=false", false)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&method=get", true)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&method=post", true)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&method=put", true)]
        [InlineData("rest://api/v1/hello?endpoint=localhost:8080&method=delete", true)]
        public void CanConstructEndpointCorrectly(string uri, bool isHttps)
        {
            var protocol = isHttps ? "https" : "http";
            var expectedHost = $"{protocol}://localhost:8080/api/v1/hello";
            var routeAttr = new RouteAttributes(uri);

            Assert.Equal(expectedHost, routeAttr.Headers.ToValidApiEndpoint());
        }

        [Fact]
        public void QueryStringConvertsParametersCorrectly()
        {
            var expected = "https://localhost:8080/api/v1/hello?name=test&id=20";
            var routeAttributes = new RouteAttributes("rest://api/v1/hello?endpoint=localhost:8080&name=test&id=20");
            var validMethods = new string[]
            {
                "endpoint",
                "Host",
                "Target"
            };
            Assert.Equal(expected, routeAttributes.Headers.ToValidApiEndpoint(validMethods));
        }
    }
}