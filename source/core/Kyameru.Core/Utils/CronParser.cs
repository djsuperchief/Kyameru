using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kyameru.Core.Utils;

sealed class CronParser
{
    // Mine so far: ^(^[0-9]$)|([1-5][0-9])|^\*$
    private static readonly Regex allowedRegex = new Regex(@"^((\d+,)+\d+|(\d+(-)\d+)|\d+|\*)$", RegexOptions.Compiled);
    private static readonly string[] allowedMonths = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];

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
        return ValidateMinutes(cron[0])
            && ValidateHours(cron[1])
            && ValidateDays(cron[2])
            && ValidateMonth(cron[3])
            && cron.Length == 5;
    }

    private static bool ValidateMonth(string month)
    {
        try
        {
            return ValidateRange(1, 12, month, allowedRegex)
                || ValidateMatchedStrings(allowedMonths, month);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateDays(string day)
    {
        try
        {
            return ValidateRange(1, 31, day, allowedRegex);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateMinutes(string minutes)
    {
        try
        {
            return ValidateRange(0, 59, minutes, allowedRegex);

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
            return ValidateRange(0, 23, hours, allowedRegex);
        }
        catch
        {
            return false;
        }
    }

    private static bool ValidateMatchedStrings(string[] items, string expression)
    {
        var test = (string expression) =>
        {
            return expression.Trim().Length == 3 && items.Any(x => x == expression.Trim());
        };

        if (expression.Contains("-"))
        {
            var strings = expression.Split("-");
            if (strings.Length > 2)
            {
                return false;
            }

            foreach (var item in strings)
            {
                if (!test(item))
                {
                    return false;
                }
            }

            if (Array.IndexOf(items, strings[0]) >= Array.IndexOf(items, strings[1]))
            {
                return false;
            }
        }

        if (expression.Contains(","))
        {
            foreach (var item in expression.Split(","))
            {
                if (!test(item))
                {
                    return false;
                }
            }
        }

        if (!expression.Contains(",") && !expression.Contains("-"))
        {
            return expression.Trim().Length == 3 && items.Any(x => x == expression.Trim());
        }

        return true;
    }

    private static bool ValidateRange(int min, int max, string expression, Regex regex)
    {

        if (!regex.IsMatch(expression)) return false;
        if (expression.Contains("-"))
        {
            var numbers = expression.Split('-').Select(x => int.Parse(x)).ToArray();
            if (numbers.Any(x => x > max || x < min))
            {
                return false;
            }

            if (numbers[0] >= numbers[1])
            {
                return false;
            }
        }

        if (expression.Contains(","))
        {
            var numbers = expression.Split(',').Select(x => int.Parse(x.Trim())).ToArray();
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
