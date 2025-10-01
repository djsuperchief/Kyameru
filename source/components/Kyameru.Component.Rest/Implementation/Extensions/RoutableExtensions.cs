using System.Net.Http;
using System.Text.Json;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Extensions
{
    public static class RoutableExtensions
    {
        public static Routable ToJsonContent(this Routable routable)
        {
            routable.SetHeader("HttpContentType", "application/json");
            routable.SetBody(JsonSerializer.Serialize(routable.Body));
            return routable;
        }
    }
}