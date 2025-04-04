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
        /// Gets or sets a value indicating a route should execute every x unit.
        /// </summary>
        public RouteConfigSchedule ScheduleEvery { get; set; }

        /// <summary>
        /// Gets or sets a value indicating a route should be executed at x unit.
        /// </summary>
        public RouteConfigSchedule ScheduleAt { get; set; }
    }
}