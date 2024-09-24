using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kyameru.Core.Extensions;

namespace Kyameru.Core.Entities;

sealed class Cron
{
    private readonly Regex NumbersOnly = new Regex(@"^\d+$", RegexOptions.Compiled);
    public DateTime NextExecution { get; private set; }
    public DateTimeKind DateKindConfig { get; private set; }

    private readonly string[] _cron;

    protected Cron(string[] cron)
    {
        // Protected constructor.
        _cron = cron;
        NextExecution = DateTime.UtcNow;
        DateKindConfig = NextExecution.Kind;
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

        if (NumbersOnly.Match(_cron[0]).Success)
        {
            NextExecution = NextExecution.GetNextCronMinute(int.Parse(_cron[0]));
        }
    }

}
