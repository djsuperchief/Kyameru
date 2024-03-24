namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route built from config
    /// </summary>
    public class RouteConfig
    {
        public RouteConfigComponent From { get; set; }
        public RouteConfigComponent[] Process { get; set; }
        public RouteConfigComponent[] To { get; set; }
        public RouteConfigOptions Options { get; set; }
    }
}