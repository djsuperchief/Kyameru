using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File.Utilities
{
    public static class StringExtensions
    {
        public static string[] SplitPiped(this string input)
        {
            string[] response = new string[0];
            if (!string.IsNullOrWhiteSpace(input))
            {
                response = input.Split("|");
            }

            return response;
        }
    }
}