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

    [Fact]
    public void ScheduleEveryHour()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var currentTime = new DateTime(2024, 01, 01, 08, 0, 0);
        timeProvider.UtcNow.Returns(currentTime);
        timeProvider.Now.Returns(currentTime.ToLocalTime());

        Core.Utils.TimeProvider.Current = timeProvider;

        var scheduler = new Scheduler();
        scheduler.Next(TimeUnit.Hour);
        Assert.Equal(9, scheduler.NextExecution.Hour);
    }

    [Fact]
    public void MissedHourExecutionIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var next = testDate.AddHours(1);
        var expected = new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0, 0, DateTimeKind.Utc);
        var scheduler = new Scheduler();
        scheduler.Next(TimeUnit.Hour);
        Assert.Equal(expected, scheduler.NextExecution);
        // Simulate missing an execution
        simulatedTime.ClearSubstitute(ClearOptions.All);
        simulatedTime.UtcNow.Returns(testDate.AddHours(2));
        simulatedTime.Now.Returns(testDate.AddHours(2).ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        expected = expected.AddHours(2);
        scheduler.Next(TimeUnit.Hour);
        Assert.Equal(expected, scheduler.NextExecution);
    }

    [Fact]
    public void AtMinuteIsCorrect()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var currentTime = new DateTime(2024, 01, 01, 08, 0, 0);
        timeProvider.UtcNow.Returns(currentTime);
        timeProvider.Now.Returns(currentTime.ToLocalTime());

        Core.Utils.TimeProvider.Current = timeProvider;

        var scheduler = new Scheduler();
        scheduler.Next(10, TimeUnit.Minute);
        Assert.Equal(10, scheduler.NextExecution.Minute);
    }

    [Fact]
    public void AtMinuteMissedExecutionIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var expected = new DateTime(testDate.Year, testDate.Month, testDate.Day, testDate.Hour, 22, 0, 0, DateTimeKind.Utc);
        var scheduler = new Scheduler();
        scheduler.Next(22, TimeUnit.Minute);
        Assert.Equal(expected, scheduler.NextExecution);
        // Simulate missing an execution
        simulatedTime.ClearSubstitute(ClearOptions.All);
        simulatedTime.UtcNow.Returns(testDate.AddHours(2));
        simulatedTime.Now.Returns(testDate.AddHours(2).ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        expected = expected.AddHours(2);
        scheduler.Next(22, TimeUnit.Minute);
        Assert.Equal(expected, scheduler.NextExecution);
    }

    [Fact]
    public void AtHourIsCorrect()
    {
        var timeProvider = Substitute.For<ITimeProvider>();
        var currentTime = new DateTime(2024, 01, 01, 08, 0, 0);
        timeProvider.UtcNow.Returns(currentTime);
        timeProvider.Now.Returns(currentTime.ToLocalTime());

        Core.Utils.TimeProvider.Current = timeProvider;

        var scheduler = new Scheduler();
        scheduler.Next(10, TimeUnit.Hour);
        Assert.Equal(10, scheduler.NextExecution.Hour);
    }

    [Fact]
    public void AtHourMissedExecutionIsCorrect()
    {
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var expected = new DateTime(testDate.Year, testDate.Month, testDate.Day, 22, testDate.Minute, 0, 0, DateTimeKind.Utc);
        var scheduler = new Scheduler();
        scheduler.Next(22, TimeUnit.Hour);
        Assert.Equal(expected, scheduler.NextExecution);
        // Simulate missing an execution
        simulatedTime.ClearSubstitute(ClearOptions.All);
        simulatedTime.UtcNow.Returns(testDate.AddHours(2));
        simulatedTime.Now.Returns(testDate.AddHours(2).ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        expected = expected.AddHours(2);
        scheduler.Next(0, TimeUnit.Hour);
        Assert.Equal(expected, scheduler.NextExecution);
    }

    [Theory]
    [InlineData(60, TimeUnit.Minute)]
    [InlineData(24, TimeUnit.Hour)]
    public void ExceedingMaximumUnitThrowsError(int value, TimeUnit unit)
    {
        var scheduler = new Scheduler();
        var exception = Record.Exception(() => scheduler.Next(value, unit));
        Assert.NotNull(exception);
        Assert.Equal(typeof(Core.Exceptions.CoreException), exception.GetType());
        Assert.Equal("An invalid unit has been specified for schedule. Minutes 0-59, Hours 0-23", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void NextScheduleEveryXUsingScheduleObjectIsCorrect(int value)
    {
        Core.Utils.TimeProvider.Reset();
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        var schedule = new Core.Entities.Schedule(TimeUnit.Minute, value, true);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var scheduler = new Scheduler();
        var expected = new DateTime(testDate.Year, testDate.Month, testDate.Day, testDate.Hour, testDate.Minute + value, 0, 0, DateTimeKind.Utc);
        scheduler.Next(schedule);
        Assert.Equal(expected, scheduler.NextExecution);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void NextScheduleEveryXHourUsingScheduleObjectIsCorrect(int value)
    {
        Core.Utils.TimeProvider.Reset();
        var simulatedTime = Substitute.For<ITimeProvider>();
        var testDate = new DateTime(2024, 01, 01, 9, 0, 0, DateTimeKind.Utc);
        var schedule = new Core.Entities.Schedule(TimeUnit.Hour, value, true);
        simulatedTime.UtcNow.Returns(testDate);
        simulatedTime.Now.Returns(testDate.ToLocalTime());
        Core.Utils.TimeProvider.Current = simulatedTime;
        var scheduler = new Scheduler();
        var expected = new DateTime(testDate.Year, testDate.Month, testDate.Day, testDate.Hour + value, 0, 0, 0, DateTimeKind.Utc);
        scheduler.Next(schedule);
        Assert.Equal(expected, scheduler.NextExecution);
    }
}
