using System;
using Kyameru.Core.Extensions;

namespace Kyameru.Core.Entities;

sealed class Cron
{
    public DateTime NextExecution { get; private set; }

    private readonly string[] _cron;

    protected Cron(string[] cron)
    {
        // Protected constructor.
        _cron = cron;
        NextExecution = DateTime.UtcNow;
        CalculateNext();
    }

    public static Cron Create(string cron)
    {
        // Cron should already have been validated.
        return new Cron(cron.Split(" "));
    }

    private void CalculateNext()
    {
        CalculateSecond();
    }

    private void CalculateSecond()
    {
        if (_cron[0] == "*")
        {
            NextExecution = NextExecution.GetNextCronMinute();
        }
    }
}
