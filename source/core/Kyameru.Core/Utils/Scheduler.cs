using System;
using System.Collections.Generic;
using Kyameru.Core.Entities;
using Kyameru.Core.Enums;
using Kyameru.Core.Extensions;

namespace Kyameru.Core.Utils
{
    /// <summary>
    /// Utility to calculate when the next scheduled time for execution is.
    /// </summary>
    internal class Scheduler
    {
        private readonly Dictionary<TimeUnit, long> timeUnitTicks = new Dictionary<TimeUnit, long>()
        {
            { TimeUnit.Minute, 600000000 },
            { TimeUnit.Hour, 36000000000 },
        };

        private readonly Dictionary<TimeUnit, int> maxTimeUnit = new Dictionary<TimeUnit, int>()
        {
            { TimeUnit.Minute, 59 },
            { TimeUnit.Hour, 23 }
        };

        private readonly bool isUtc;

        /// <summary>
        /// Gets the next execution date.
        /// </summary>
        public DateTime NextExecution { get; private set; }

        /// <summary>
        /// Instantiates a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="isUtcTime">Value indicating if UTC should be used.</param>
        public Scheduler(bool isUtcTime = true)
        {
            isUtc = isUtcTime;
            if (isUtc)
            {
                NextExecution = Core.Utils.TimeProvider.Current.UtcNow;
            }
            else
            {
                NextExecution = Core.Utils.TimeProvider.Current.Now;
            }
        }

        internal void Next(TimeUnit unit)
        {
            Every(1, unit);
        }

        internal void Next(int at, TimeUnit unit)
        {
            IsValid(at, unit);
            NextExecution = NextExecution.SetUnit(unit, at);
            if (at >= 0 && at < maxTimeUnit[unit])
            {
                do
                {
                    NextExecution = IncreaseWholeUnit(unit, at).UpToMinute();
                } while (GetTimeProvider() >= NextExecution);
            }
        }

        internal void Every(int value, TimeUnit unit)
        {
            do
            {
                if (value <= 0)
                {
                    value = 1;
                }

                NextExecution = NextExecution.AddTicks(timeUnitTicks[unit] * value);
            } while (GetTimeProvider() >= NextExecution);

            NextExecution = NextExecution.UpToMinute();
        }

        internal void Next(Schedule schedule)
        {
            if (schedule.IsEvery)
            {
                Every(schedule.Value, schedule.Unit);
            }
            else
            {
                Next(schedule.Value, schedule.Unit);
            }
        }

        private DateTime GetTimeProvider()
        {
            return isUtc ? TimeProvider.Current.UtcNow : TimeProvider.Current.Now;
        }

        private DateTime IncreaseWholeUnit(TimeUnit unit, int at)
        {
            DateTime newDate = NextExecution;
            if (unit == TimeUnit.Minute)
            {
                newDate = GetTimeProvider() >= NextExecution ? NextExecution.AddHours(1) : NextExecution;
            }
            else
            {
                newDate = GetTimeProvider() >= NextExecution ? NextExecution.AddDays(1) : NextExecution;
            }

            return newDate.SetUnit(unit, at);
        }

        private void IsValid(int value, TimeUnit unit)
        {
            if (value > maxTimeUnit[unit])
            {
                throw new Exceptions.CoreException(Resources.ERROR_SCHEDULE_TIME_INVALID);
            }
        }
    }
}
