using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kyameru.Core.Extensions;
using Kyameru.Core.Utils;

namespace Kyameru.Core.Entities;

public class Cron
{
    private readonly Regex NumbersOnly = new Regex(@"^\d+$", RegexOptions.Compiled);
    public DateTime NextExecution { get; private set; }
    public DateTimeKind DateKindConfig { get; private set; }

    private readonly string[] _cron;

    public Cron(string[] cron)
    {
        // Protected constructor.
        _cron = cron;
        NextExecution = TimeProvider.Current.UtcNow;
        DateKindConfig = NextExecution.Kind;
        CalculateNext();
    }

    public static Cron Create(string cron)
    {
        // Cron should already have been validated.
        return new Cron(cron.Split(" "));
    }

    public void Next()
    {
        CalculateNext();
    }

    private void CalculateNext()
    {
        CalculateMinute();
    }

    private void CalculateMinute()
    {
        if (_cron[0] == "*")
        {
            NextExecution = NextExecution.GetNextCronMinute();
        }

        if (NumbersOnly.Match(_cron[0]).Success)
        {
            NextExecution = NextExecution.GetCronAtMinute(int.Parse(_cron[0]));
        }

        if (_cron[0].Contains('-'))
        {
            var expression = _cron[0].Split('-').Select(x => int.Parse(x.Trim())).ToArray();
            NextExecution = NextExecution.GetCronMinuteBetween(expression[0], expression[1]);
        }
    }

}
