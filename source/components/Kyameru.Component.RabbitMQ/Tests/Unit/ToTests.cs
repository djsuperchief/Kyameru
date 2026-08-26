using Kyameru.Component.RabbitMQ.Entities;
using Kyameru.Core.Entities;

namespace Kyameru.Component.RabbitMQ.Tests;

public class ToTests
{
    [Theory]
    [InlineData("rabbitmq://guest:guest@localhost/exchange?queue=test", true)]
    [InlineData("rabbitmq://guest:guest@localhost/exchange", false)]
    public void RabbitMQUriFormatIsCorrect(string uri, bool queue)
    {
        var routeAttribute = new RouteAttributes(uri);
        var connection = RabbitMQConnection.FromRouteAttributes(routeAttribute);
        
        Assert.Equal("guest", connection.Username);
        Assert.Equal("guest", connection.Password);
        Assert.Equal("localhost", connection.Host);
        Assert.Equal("exchange", connection.Exchange);
        if (queue)
        {
            Assert.Equal("test", connection.Queue);
        }
    }
}