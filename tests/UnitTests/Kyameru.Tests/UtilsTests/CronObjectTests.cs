using System;
using Kyameru.Core.Utils;
using Kyameru.Core.Extensions;
using Xunit;
using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using Kyameru.Core.Contracts;
using NSubstitute.ClearExtensions;

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
    public void AtXMinuteIsCorrect(int minute)
    {
        var cron = $"{minute} * * * *";
        var current = DateTime.UtcNow;
        var expectedHour = current.Minute >= minute ? current.AddHours(1).Hour : current.Hour;

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

    [Fact]
    public void GetMinuteAt()
    {
        var cron = "1 * * * *";
        var now = DateTime.UtcNow;
        var current = new DateTime(now.Year, now.Month, now.Day, now.Hour - 1, 1, 0, 0, DateTimeKind.Utc);
        var expected = new DateTime(current.Year, current.Month, current.Day, now.Hour + 1, 1, 0, 0, DateTimeKind.Utc);

        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.True(isValid);
        Assert.Equal(expected, cronObject.NextExecution);
        Assert.Equal(DateTimeKind.Utc, cronObject.DateKindConfig);

    }

    [Fact]
    public void SecondMinuteExecutionIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var cron = "* * * * *";
        var next = testDate.AddMinutes(1);
        var expected = new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0, 0, DateTimeKind.Utc);
        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.Equal(expected, cronObject.NextExecution);
        // Simulate missing an execution
        simulatedTime.ClearSubstitute(ClearOptions.All);
        simulatedTime.UtcNow.Returns(testDate.AddMinutes(2));
        simulatedTime.Now.Returns(testDate.AddMinutes(2).ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        expected = expected.AddMinutes(2);
        cronObject.Next();
        Assert.Equal(expected, cronObject.NextExecution);
    }

    [Fact]
    public void MinuteOverHourIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 59, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var cron = "* * * * *";
        var expected = new DateTime(2024, 01, 01, 10, 0, 0, DateTimeKind.Utc);

        var (isValid, cronObject) = CronParser.ValidateCron(cron);
        Assert.Equal(expected, cronObject.NextExecution);
    }

    public static IEnumerable<object[]> MinuteThroughMinuteTestData()
    {
        yield return new object[] { 4, 36, new DateTime(2024, 09, 01, 9, 0, 0) };
    }
}
