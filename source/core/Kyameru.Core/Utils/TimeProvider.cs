using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Core.Utils
{
    internal class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
