using Kyameru.Core.Entities;

namespace Kyameru.Component.RabbitMQ.Entities
{
    public class RabbitMQConnection
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }

        public static RabbitMQConnection FromRouteAttributes(RouteAttributes routeAttributes)
        {
            var response = new RabbitMQConnection()
            {
                Username = routeAttributes.Headers["UserName"],
                Password = routeAttributes.Headers["Password"],
                Exchange = routeAttributes.Headers["Target"].Replace("/", ""),
                Host = routeAttributes.Headers["Host"]
            };

            if (routeAttributes.Headers.TryGetValue("queue", out var header))
            {
                response.Queue = header;
            }
            
            return response;
        }
    }
}