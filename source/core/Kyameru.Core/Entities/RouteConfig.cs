using System.IO;
using System.Text.Json;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route built from config
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Gets or sets the from component.
        /// </summary>
        public RouteConfigComponent From { get; set; }
        
        /// <summary>
        /// Gets or sets the list of processing components.
        /// </summary>
        public string[] Process { get; set; }
        
        /// <summary>
        /// Gets or sets the list of to components.
        /// </summary>
        public RouteConfigComponent[] To { get; set; }
        
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public RouteConfigOptions Options { get; set; }

        /// <summary>
        /// Loads the route from a config file.
        /// </summary>
        /// <param name="fileLocation">Full file path of the config file.</param>
        /// <returns>Returns an instance of the <see cref="RouteConfig"/> class.</returns>
        public static RouteConfig Load(string fileLocation)
        {
            var file = File.ReadAllText(fileLocation);
            return JsonSerializer.Deserialize<RouteConfig>(file);
        }
    }
}