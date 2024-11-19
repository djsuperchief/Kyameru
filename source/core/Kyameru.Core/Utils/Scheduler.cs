using System;
using System.Collections.Generic;
using Kyameru.Core.Enums;

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
            { TimeUnit.Hour, 36000000000 }
        };

        /// <summary>
        /// Gets the next execution date.
        /// </summary>
        public DateTime NextExecution { get; private set; }

        public Scheduler()
        {
            NextExecution = Core.Utils.TimeProvider.Current.UtcNow;
        }

        internal void Next(TimeUnit unit)
        {
            var totalIncrease = NextExecution;
            do
            {
                totalIncrease = totalIncrease.AddTicks(timeUnitTicks[unit]);
            } while (TimeProvider.Current.UtcNow >= totalIncrease);

            NextExecution = UpToMinute(totalIncrease);
        }

        private static DateTime UpToMinute(DateTime input)
        {
            // Standard does not have access to Microseconds and as such any date time will also include it.
            // For the sake of simplicity, this seems fine.
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0, 0, input.Kind);
        }
    }
}
