using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Ftp.Extensions
{
    /// <summary>
    /// String extensions class.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the string contains an empty path or is empty.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns><Returns a value indicating whether the check was successful.</returns>
        public static bool IsNullOrEmptyPath(this string input)
        {
            if (input == "/" || string.IsNullOrWhiteSpace(input))
            {
                return true;
            }

            return false;
        }

        public static string StripEndingSlash(this string input)
        {
            return input.Substring(input.Length - 1, 1) == "/" ? input.Substring(0, input.Length - 1) : input;
        }
    }
}