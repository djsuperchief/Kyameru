using System;
using Kyameru.Core.Utils;

namespace Kyameru.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime SetMinute(this DateTime input, int minute = 0)
    {
        if (input.Minute > minute || input.Minute < minute)
        {
            input = input.AddMinutes(minute - input.Minute);
        }

        return input;
    }

    /// <summary>
    /// Gets the date and time up to and including the minute but excludes seconds, milliseconds etc.
    /// </summary>
    /// <param name="input">DateTime for input.</param>
    /// <returns>DateTime up to minute.</returns>
    public static DateTime UpToMinute(this DateTime input)
    {
        // Standard does not have access to Microseconds and as such any date time will also include it.
        // For the sake of simplicity, this seems fine.
        return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, 0, input.Kind);
    }
}
