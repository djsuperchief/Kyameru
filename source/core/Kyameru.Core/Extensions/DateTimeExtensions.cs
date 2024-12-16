using System;
using Kyameru.Core.Enums;

namespace Kyameru.Core.Extensions
{
    /// <summary>
    /// Extensions for date and time.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the date time up to the minute excluding milliseconds.
        /// </summary>
        /// <remarks>
        /// The latest API for date time would allow us to exclude milliseconds but not in standard.
        /// </remarks>
        /// <param name="input">DateTime</param>
        /// <returns>Returns a <see cref="DateTime"/> without milliseconds.</returns>
        public static DateTime UpToMinute(this DateTime input)
        {
            // Standard does not have access to Microseconds and as such any date time will also include it.
            // For the sake of simplicity, this seems fine.
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, 0, input.Kind);
        }

        /// <summary>
        /// Sets the date time to a specific time unit value.
        /// </summary>
        /// <param name="input">DateTime</param>
        /// <param name="unit">Unit of time.</param>
        /// <param name="value">Value to set.</param>
        /// <returns>Returns a <see cref="DateTime"/> at the exact minute / hour specified.</returns>
        public static DateTime SetUnit(this DateTime input, TimeUnit unit, int value)
        {
            if (unit == TimeUnit.Minute)
            {
                return input.Date + new TimeSpan(input.Hour, value, input.Second);
            }
            else
            {
                return input.Date + new TimeSpan(value, input.Minute, input.Second);
            }
        }
    }
}
