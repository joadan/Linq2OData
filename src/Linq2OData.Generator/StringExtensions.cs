using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace Linq2OData.Generator;

internal static class StringExtensions
{
    private static readonly char[] PropertyNameSeparators = [' ', '/', '-'];

    private static bool StartsWith_3rd(StringBuilder result)
    {
        return result[0] == '3' && result[1] == 'r' && result[2] == 'd';
    }

    extension(string text)
    {

        public string SafeVariableName(string? enclosingTypeName = null)
        {
            if (IsCSharpKeyword(text)) {
                return $"@{text}";
            }

            if (enclosingTypeName != null && text == enclosingTypeName) {
                return $"{text}_";
            }

            return text;

        }

        public  bool IsCSharpKeyword()
        {
            return SyntaxFacts.GetKeywordKind(text) != SyntaxKind.None
                || SyntaxFacts.GetContextualKeywordKind(text) != SyntaxKind.None;
        }

        public string ToValidCSharpClassName()
        {
            var isValid = SyntaxFacts.IsValidIdentifier(text);

            if (!isValid)
            {
                // File name contains invalid chars, remove them
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                text = regex.Replace(text, "");

                // Class name doesn't begin with a letter, insert an underscore
                if (!char.IsLetter(text, 0))
                {
                    text = text.Insert(0, "_");
                }
            }

            return text.Replace(" ", string.Empty);
        }

        internal string ToValidCSharpPascalCase()
        {
            return text.ToValidCSharpIdentifier().ToUpperFirst();
        }

        internal string ToValidCSharpCamelCaseParameterName()
        {
            return $"@{text.ToValidCSharpIdentifier().ToLowerFirst()}";
        }

        internal string ToValidCSharpCamelCase()
        {
            return text.ToValidCSharpIdentifier().ToLowerFirst();
        }

        internal string ToLowerFirst()
        {
            return char.ToLowerInvariant(text[0]) + text[1..];
        }

        internal string ToUpperFirst()
        {
            return char.ToUpperInvariant(text[0]) + text[1..];
        }

        private string ToValidCSharpIdentifier()
        {
            if (SyntaxFacts.IsValidIdentifier(text))
            {
                return text;
            }

            var words = text.Split(PropertyNameSeparators, StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();
            foreach (var word in words)
            {
                if (char.IsAsciiDigit(word[0]))
                {
                    result.Append((ReadOnlySpan<char>)CultureInfo.CurrentCulture.TextInfo.ToLower(word));
                    continue;
                }

                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower()));
            }

            for (var i = 0; i < result.Length; i++)
            {
                if (char.IsLetterOrDigit(result[i]))
                {
                    continue;
                }

                result.Remove(i, 1);
                i--;
            }

            if (StartsWith_3rd(result))
            {
                result.Replace("3rd", "Third", 0, 3);
            }

            if (char.IsAsciiDigit(result[0]))
            {
                result.Insert(0, "_");
            }

            return result.ToString();
        }
    }
}