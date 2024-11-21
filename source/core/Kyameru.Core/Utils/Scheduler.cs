using System;
using System.Collections.Generic;
using Kyameru.Core.Enums;
using Kyameru.Core.Extensions;

namespace Kyameru.Core.Utils
{
    /// <summary>
    /// Utility to calculate when the next scheduled time for execution is.
    /// </summary>
    public class Scheduler
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
            do
            {
                NextExecution = NextExecution.AddTicks(timeUnitTicks[unit]);
            } while (GetTimeProvider() >= NextExecution);

            NextExecution = NextExecution.UpToMinute();
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
