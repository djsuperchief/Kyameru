using System;
using Kyameru.Core.Utils;
using Kyameru.Core.Extensions;
using Xunit;

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
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void EveryXMinuteIsCorrect(int minute)
    {
        var cron = $"{minute} * * * *";
        var current = DateTime.UtcNow;
    }
}
