using System;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route config schedule.
    /// </summary>
    public class RouteConfigSchedule
    {
        /// <summary>
        /// Gets or sets a value indicating the unit of time.
        /// </summary>
        public Enums.TimeUnit TimeUnit { get; set; }

        /// <summary>
        /// Gets or sets a value for the time unit.
        /// </summary>
        public int Value { get; set; }
    }
}
