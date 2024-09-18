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

    [Theory]
    [MemberData(nameof(DayOfWeekTestData))]
    public void DayOfWeekIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(DayOfWeekStringTestData))]
    public void DayOfWeekStringIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(MinuteStepTestData))]
    public void MinuteStepIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(HourStepTestData))]
    public void HourStepIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(DayOfMonthStepTestData))]
    public void DayOfMonthStepIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(MonthStepTestData))]
    public void MonthStepIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    [Theory]
    [MemberData(nameof(DayOfWeekStepTestData))]
    public void DayOfWeekStepIsValid(string cron, bool isValid)
    {
        Assert.Equal(isValid, CronParser.ValidateCron(cron).Item1);
    }

    public static IEnumerable<object[]> MinuteStepTestData()
    {
        return GenerateStepData(0, 60, "{0} * * * *");
    }

    public static IEnumerable<object[]> HourStepTestData()
    {
        return GenerateStepData(0, 24, "* {0} * * *");
    }

    public static IEnumerable<object[]> DayOfMonthStepTestData()
    {
        return GenerateStepData(1, 32, "* * {0} * *");
    }

    public static IEnumerable<object[]> MonthStepTestData()
    {
        return GenerateStepData(1, 13, "* * * {0} *");
    }

    public static IEnumerable<object[]> DayOfWeekStepTestData()
    {
        return GenerateStepData(0, 7, "* * * * {0}");
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

    public static IEnumerable<object[]> DayOfWeekTestData()
    {
        return GenerateRangeTestData(0, 7, "* * * * {0}");
    }

    public static IEnumerable<object[]> MonthStringTestData()
    {
        string[] months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];

        return GenerateStringTestData(months, "* * * {0} *");
    }

    public static IEnumerable<object[]> DayOfWeekStringTestData()
    {
        string[] days = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
        return GenerateStringTestData(days, "* * * * {0}");
    }

    public static IEnumerable<object[]> GenerateStringTestData(string[] testItems, string expression)
    {
        for (var i = 0; i < testItems.Length; i++)
        {
            yield return new object[] { string.Format(expression, testItems[i]), true };
        }

        yield return new object[] { string.Format(expression, "TES"), false };
        yield return new object[] { string.Format(expression, "TEST"), false };
        for (var i = 1; i < testItems.Length; i++)
        {
            for (var x = 1; x < testItems.Length; x++)
            {
                bool isValid = x > i;
                yield return new object[] { string.Format(expression, $"{testItems[i]}-{testItems[x]}"), isValid };
            }
        }

        for (var i = 1; i < testItems.Length; i++)
        {
            for (var x = 1; x < testItems.Length; x++)
            {
                bool isValid = true;
                yield return new object[] { string.Format(expression, $"{testItems[i]},{testItems[x]}"), isValid };
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

    public static IEnumerable<object[]> GenerateStepData(int min, int max, string expression)
    {
        // Whilst generating every combination is a good thing, it doesn't really add anything.
        // For this reason, these tests just ensure that rules are adhered to.
        yield return new object[] { string.Format(expression, $"*/{min}"), false };
        yield return new object[] { string.Format(expression, $"{min}/{max}"), false };

        yield return new object[] { string.Format(expression, $"{min}/{max - 1}"), true };
        yield return new object[] { string.Format(expression, $"*/{max - 1}"), true };

        yield return new object[] { string.Format(expression, $"{min + 1}/{max - 1}"), true };
        yield return new object[] { string.Format(expression, $"{max - 1}/{min + 1}"), true };
        yield return new object[] { string.Format(expression, $"{max - 1}/{min}"), false };
        // for (var i = min; i <= max; i++)
        // {
        //     var valid = i < max && i > min;
        //     yield return new object[] { string.Format(expression, $"*/{i}"), valid };
        // }

        // yield return new object[] { string.Format(expression, $"{min}/*"), false };

        // for (var i = min; i < max; i++)
        // {
        //     for (var x = 0; x < max; x++)
        //     {
        //         var valid = i < max && x < max && i >= min && x >= min;
        //         yield return new object[] { string.Format(expression, $"{i}/{x}"), valid };
        //     }
        // }
    }

    public static IEnumerable<object[]> CommaTestData()
    {

        for (var position = 0; position < 5; position++)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 5; i++)
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
