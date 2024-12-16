using System;
using Kyameru.Core.Enums;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Schedule object for storing route timing calculation..
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Gets the unit of time.
        /// </summary>
        public TimeUnit Unit { get; }

        /// <summary>
        /// Gets the value for the unit of time.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets a value indicating whether the time is every (x) unit.
        /// </summary>
        public bool IsEvery { get; set; }

        /// <summary>
        /// Instantiates a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="unit">Unit of time.</param>
        /// <param name="value">Value of time.</param>
        /// <param name="isEvery">Value indicating whether the time is every (x) unit.</param>
        public Schedule(TimeUnit unit, int value, bool isEvery)
        {
            Unit = unit;
            Value = value;
            IsEvery = isEvery;
        }
    }
}
