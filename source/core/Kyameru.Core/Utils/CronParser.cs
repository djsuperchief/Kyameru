using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kyameru.Core.Utils;

sealed class CronParser
{
    // Mine so far: ^(^[0-9]$)|([1-5][0-9])|^\*$
    private static readonly Regex minutesHoursRegex = new Regex(@"^((\d+,)+\d+|(\d+(-)\d+)|\d+|\*)$", RegexOptions.Compiled);
    private static readonly char[] allowedChars = ['*', ',', '-'];
    public static (bool, Entities.Cron) ValidateCron(string cron)
    {
        var isValid = CronIsValid(cron.Split(" "));
        if (isValid)
        {

        }

        return (isValid, null);
    }

    private static bool CronIsValid(string[] cron)
    {
        return ValidateMinutes(cron[0]) && ValidateHours(cron[1]);
    }

    private static bool ValidateMinutes(string minutes)
    {
        try
        {
            return ValidateRange(0, 59, minutes, minutesHoursRegex);

        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateHours(string hours)
    {
        try
        {
            return ValidateRange(0, 23, hours, minutesHoursRegex);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateRange(int min, int max, string expression, Regex regex)
    {

        if (!regex.IsMatch(expression)) return false;
        if (expression.Contains("-"))
        {
            var numbers = expression.Split('-').Select(x => int.Parse(x));
            if (numbers.Any(x => x > max || x < min))
            {
                return false;
            }
        }

        if (expression.Contains(","))
        {
            var numbers = expression.Split(',').Select(x => int.Parse(x.Trim()));
            if (numbers.Any(x => x > max || x < min))
            {
                return false;
            }
        }

        if (int.TryParse(expression, out var number))
        {
            if (number > max || number < min)
            {
                return false;
            }
        }

        return true;
    }
}
