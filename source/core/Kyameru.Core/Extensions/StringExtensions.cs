using System.Linq;

namespace Kyameru.Core.Extensions
{
    /// <summary>
    /// String extensions.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Makes the first letter of a string uppercase.
        /// </summary>
        /// <param name="input">String to process.</param>
        /// <returns>Returns the input with the first letter as uppercase.</returns>
        public static string ToFirstCaseUpper(this string input)
        {
            return $"{input.First().ToString().ToUpper()}{input.Substring(1)}";
        }
    }
}