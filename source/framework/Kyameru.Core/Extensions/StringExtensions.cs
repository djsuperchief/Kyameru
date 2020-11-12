﻿using System.Linq;

namespace Kyameru.Core.Extensions
{
    internal static class StringExtensions
    {
        public static string ToFirstCaseUpper(this string input)
        {
            return $"{input.First().ToString().ToUpper()}{input.Substring(1)}";
        }
    }
}