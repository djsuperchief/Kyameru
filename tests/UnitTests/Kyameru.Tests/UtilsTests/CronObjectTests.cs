using System;
using Kyameru.Core.Utils;
using Kyameru.Core.Extensions;
using Xunit;
using System.Collections;
using System.Collections.Generic;

namespace Kyameru.Tests.UtilsTests;

public class CronObjectTests
{
    [Fact]
    public void EveryMinuteIsCorrect()
    {
        var cron = "* * * * *";
        var current = DateTime.UtcNow;
        var expected = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.AddMinutes(1).Minute, 0, 0, DateTimeKind.Utc);

        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.True(isValid);
        Assert.Equal(expected, cronObject.NextExecution);
        Assert.Equal(DateTimeKind.Utc, cronObject.DateKindConfig);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void EveryXMinuteIsCorrect(int minute)
    {
        var cron = $"{minute} * * * *";
        var current = DateTime.UtcNow;
        var expectedHour = current.Minute > minute ? current.AddHours(1).Hour : current.Hour;

        var expected = new DateTime(current.Year, current.Month, current.Day, expectedHour, minute, 0, 0, DateTimeKind.Utc);
        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.True(isValid);
        Assert.Equal(expected, cronObject.NextExecution);
    }


    [Theory]
    [MemberData(nameof(MinuteThroughMinuteTestData))]

    public void MinuteThroughMinuteIsCorrect(int min, int max, DateTime current)
    {
        var cron = $"{min}-{max} * * * *";
        var expectedMinute = min;
        if (current.Minute >= min)
        {
            expectedMinute = current.Minute + 1;
        }

        if (current.Minute >= max)
        {
            expectedMinute = min;
        }

        var expected = new DateTime(current.Year, current.Month, current.Day, current.Hour, expectedMinute, 0, 0, DateTimeKind.Utc);
        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.True(isValid);
        Assert.Equal(expected, cronObject.NextExecution);
    }

    public static IEnumerable<object[]> MinuteThroughMinuteTestData()
    {
        yield return new object[] { 4, 36, new DateTime(2024, 09, 01, 9, 0, 0) };
    }
}
