using System;

namespace Kyameru.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetNextCronMinute(this DateTime input)
    {
        return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.AddMinutes(1).Minute, 0, 0);
    }
}
