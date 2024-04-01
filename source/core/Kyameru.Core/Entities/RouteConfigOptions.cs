namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route options.
    /// </summary>
    public class RouteConfigOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if the route should bubble exceptions.
        /// </summary>
        public bool RaiseExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the route should be built async.
        /// </summary>
        public bool BuildAsync { get; set; }
    }
}