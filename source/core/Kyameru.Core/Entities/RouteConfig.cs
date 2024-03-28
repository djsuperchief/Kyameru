using System.IO;
using System.Text.Json;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route built from config
    /// </summary>
    public class RouteConfig
    {
        public RouteConfigComponent From { get; set; }
        public string[] Process { get; set; }
        public RouteConfigComponent[] To { get; set; }
        public RouteConfigOptions Options { get; set; }

        public static RouteConfig Load(string fileLocation)
        {
            var file = File.ReadAllText(fileLocation);
            return JsonSerializer.Deserialize<RouteConfig>(file);
        }
    }
}