using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Kyameru.Core.Utils;
using NSubstitute;
using Xunit;

namespace Kyameru.Tests.UtilsTests;

public class CronParserTests
{
    [Theory]
    [InlineData("test", false)]
    [InlineData("0 1 * * *", true)]
    [InlineData("0 1 * * * *", false)]
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

    [Theory]
    [MemberData(nameof(MonthOfYearTestData))]
    public void MonthOfYearIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(MonthStringTestData))]
    public void MonthOfYearStringIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(CommaTestData))]
    public void GeneralCommaIsValid(string cron)
    {
        Assert.True(CronParser.ValidateCron(cron).Item1);
    }

    public static IEnumerable<object[]> MonthOfYearTestData()
    {
        return GenerateRangeTestData(1, 13, "* * * {0} *");
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
        return GenerateRangeTestData(0, 60, "{0} * * * *");
    }

    public static IEnumerable<object[]> MonthStringTestData()
    {
        string[] months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
        for (var i = 0; i < months.Length; i++)
        {
            yield return new object[] { $"* * * {months[i]} *", true };
        }
        yield return new object[] { $"* * * TES *", false };
        yield return new object[] { $"* * * TEST *", false };
        for (var i = 1; i < months.Length; i++)
        {
            for (var x = 1; x < months.Length; x++)
            {
                bool isValid = x > i;
                yield return new object[] { string.Format("* * * {0} *", $"{months[i]}-{months[x]}"), isValid };
            }
        }

        for (var i = 1; i < months.Length; i++)
        {
            for (var x = 1; x < months.Length; x++)
            {
                bool isValid = true;
                yield return new object[] { string.Format("* * * {0} *", $"{months[i]},{months[x]}"), isValid };
            }
        }
    }

    public static IEnumerable<object[]> GenerateRangeTestData(int min, int max, string expression)
    {
        for (var i = min; i <= max; i++)
        {
            var valid = i < max;
            yield return new object[] { string.Format(expression, i), valid };
        }
        yield return new object[] { $"* * * * *", true };

        for (var i = min; i < max; i++)
        {
            for (var x = min; x < max; x++)
            {
                var valid = i < max && x < max;
                if (valid)
                {
                    valid = x > i;
                }

                yield return new object[] { string.Format(expression, $"{i}-{x}"), valid };
            }
        }

        for (var i = min; i < max; i++)
        {
            for (var x = 0; x < max; x++)
            {
                var valid = i < max && x < max && i >= min && x >= min;
                yield return new object[] { string.Format(expression, $"{i},{x}"), valid };
            }
        }

        yield return new object[] { string.Format(expression, $"{min},{max - 1}"), true };
        yield return new object[] { string.Format(expression, $"{min},{max}"), false };
    }

    public static IEnumerable<object[]> CommaTestData()
    {

        for (var position = 0; position < 6; position++)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 6; i++)
            {
                if (i == position)
                {
                    builder.Append("1,2,3,5,4 ");
                }
                else
                {
                    builder.Append("* ");
                }
            }

            yield return new object[] { builder.ToString().TrimEnd() };

        }
    }
}
