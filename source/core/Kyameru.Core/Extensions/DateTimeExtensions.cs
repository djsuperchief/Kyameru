using System;
using Kyameru.Core.Enums;

namespace Kyameru.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UpToMinute(this DateTime input)
        {
            // Standard does not have access to Microseconds and as such any date time will also include it.
            // For the sake of simplicity, this seems fine.
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, 0, input.Kind);
        }

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
