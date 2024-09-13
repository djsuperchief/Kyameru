using System;
using System.Collections;
using System.Collections.Generic;
using Kyameru.Core.Utils;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.UtilsTests;

public class CronParserTests
{
    [Theory]
    [InlineData("test", false)]
    [InlineData("0 1 * * *", true)]
    public void IsValidCron(string cron, bool valid)
    {
        var (isValid, result) = CronParser.ValidateCron(cron);
        Assert.Equal(valid, isValid);
    }

    [Theory]
    [MemberData(nameof(MinuteTestData))]
    public void MinutesAreValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(HourTestData))]
    public void HoursAreValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(DayOfMonthTestData))]
    public void DayOfMonthIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    public static IEnumerable<object[]> DayOfMonthTestData()
    {
        return GenerateRangeTestData(1, 32, "* * {0} * *");
    }

    public static IEnumerable<object[]> HourTestData()
    {
        return GenerateRangeTestData(0, 24, "* {0} * * *");
    }

    public static IEnumerable<object[]> MinuteTestData()
    {
        return GenerateRangeTestData(0, 60, "{0} * * *");
    }

    public static IEnumerable<object[]> GenerateRangeTestData(int min, int max, string expression)
    {
        for (var i = min; i < max; i++)
        {
            var valid = i < max;
            yield return new object[] { string.Format(expression, i), valid };
        }
        yield return new object[] { $"* * * * *", true };

        for (var i = min; i < max; i++)
        {
            for (var x = i + 1; x < max; x++)
            {
                var valid = i < max && x < max;
                yield return new object[] { string.Format(expression, $"{i}-{x}"), valid };
            }
        }

        yield return new object[] { string.Format(expression, $"{min},{max - 1}"), true };
        yield return new object[] { string.Format(expression, $"{min},{max}"), false };
    }
}
