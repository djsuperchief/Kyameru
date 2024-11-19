using NSubstitute;
using Kyameru.Core.Contracts;
using Xunit;
using System;
using Kyameru.Core.Utils;
using Kyameru.Core.Enums;
using NSubstitute.ClearExtensions;

namespace Kyameru.Tests.Schedule;

public class ScheduleTests
{
    [Fact]
    public void ScheduleEveryMinute()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var currentTime = new DateTime(2024, 01, 01, 08, 0, 0);
        timeProvider.UtcNow.Returns(currentTime);
        timeProvider.Now.Returns(currentTime.ToLocalTime());

        Core.Utils.TimeProvider.Current = timeProvider;

        var scheduler = new Scheduler();
        scheduler.Next(TimeUnit.Minute);
        Assert.Equal(1, scheduler.NextExecution.Minute);
    }

    [Fact]
    public void SecondMinuteExecutionIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var next = testDate.AddMinutes(1);
        var expected = new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0, 0, DateTimeKind.Utc);
        var scheduler = new Scheduler();
        scheduler.Next(TimeUnit.Minute);
        Assert.Equal(expected, scheduler.NextExecution);
        // Simulate missing an execution
        simulatedTime.ClearSubstitute(ClearOptions.All);
        simulatedTime.UtcNow.Returns(testDate.AddMinutes(2));
        simulatedTime.Now.Returns(testDate.AddMinutes(2).ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        expected = expected.AddMinutes(2);
        scheduler.Next(TimeUnit.Minute);
        Assert.Equal(expected, scheduler.NextExecution);
    }
}
