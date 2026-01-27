using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Generator.Models
{
    internal static class Helpers
    {
        internal static string ToCamelCaseVariable(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int len = input.Length;
            int i = 0;

            // Find consecutive uppercase letters at the start
            while (i < len && char.IsUpper(input[i]))
                i++;

            // If first char is the only uppercase, just lowercase it
            if (i == 1)
                return char.ToLower(input[0]) + input.Substring(1);

            // If the whole string is uppercase (like "ID"), lowercase everything
            if (i == len)
                return input.ToLower();

            // Lowercase all leading uppercase letters except the last if next char is lowercase
            // Handles cases like "XMLHttpRequest" -> "xmlHttpRequest"
            int endOfAcronym = i;
            if (i > 1 && i < len && char.IsLower(input[i]))
                endOfAcronym = i - 1;

            return input.Substring(0, endOfAcronym).ToLower() + input.Substring(endOfAcronym);
        }

    }
}
