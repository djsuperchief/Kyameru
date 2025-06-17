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
        public void CanConstructEndpointCorrectly(string uri, bool isHttps)
        {
            var protocol = isHttps ? "https" : "http";
            var expectedHost = $"{protocol}://localhost:8080/api/v1/hello";
            var routeAttr = new RouteAttributes(uri);

            Assert.Equal(expectedHost, routeAttr.ToValidApiEndpoint());
        }
    }
}