using System;
using Kyameru.Core.Enums;

namespace Kyameru.Core.Entities
{
    internal class Schedule
    {
        public TimeUnit Unit { get; }

        public int Value { get; }

        public bool IsEvery { get; set; }

        public Schedule(TimeUnit unit, int value, bool isEvery)
        {
            Unit = unit;
            Value = value;
            IsEvery = isEvery;
        }
    }
}
