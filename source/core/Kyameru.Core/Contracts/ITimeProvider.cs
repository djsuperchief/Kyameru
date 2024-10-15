using System;

namespace Kyameru.Core.Contracts;

public interface ITimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
