using System;
using Kyameru.Core.Utils;

namespace Kyameru.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetNextCronMinute(this DateTime input)
    {
        DateTime nextDate = input;
        do
        {
            nextDate = nextDate.AddMinutes(1);
            nextDate = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, nextDate.Hour, nextDate.Minute, 0, 0);
        } while (TimeProvider.Current.UtcNow >= nextDate);
        // we need to take the current time into account (and date) because the input could be behind and assuming it is right leads to problems.
        return nextDate;
    }


    public static DateTime GetCronAtMinute(this DateTime input, int minute = 0)
    {
        var hour = input.Hour;
        if (input.Minute >= minute)
        {
            hour++;
        }

        return new DateTime(input.Year, input.Month, input.Day, hour, minute, 0, 0);
    }

    public static DateTime GetCronMinuteBetween(this DateTime input, int first, int second)
    {
        throw new NotImplementedException();
    }
}
