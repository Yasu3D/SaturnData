using System.Text;
using System.Text.RegularExpressions;

namespace SaturnData.Content.Serialization;

public static class ContentSerializationHelpers
{
    /// <summary>
    /// Converts a <c>PascalCase</c> string into <c>MACRO_CASE</c>.
    /// </summary>
    /// <param name="s">The <c>PascalCase</c> string to convert.</param>
    /// <returns>Returns a string in <c>MACRO_CASE</c></returns>
    /// <remarks>Abbreviations like "CSV" will be forcibly split, because <c>ToPascalCase()</c> would blindly convert them back to "Csv" otherwise.</remarks>
    public static string PascalCaseToMacroCase(this string s)
    {
        return ToMacroCaseRegex.Replace(s, "_$1").ToUpperInvariant();
    }
    
    public static string Indent(this string s, int step = 1)
    {
        return new string(' ', 4 * step) + s;
    }

    public static int GetSpacesAtStart(this string s)
    {
        int spaces = 0;
        foreach (char c in s)
        {
            if (c != ' ') break;
            spaces++;
        }
        
        return spaces;
    }
    
    public static string Escape(this string s)
    {
        StringBuilder result = new();

        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': result.Append("\\\\"); break;
                case '"': result.Append("\\\""); break;
                case '\n': result.Append("\\n"); break;
                case '\r': result.Append("\\r"); break;
                default: result.Append(c); break;
            }
        }

        return result.ToString();
    }

    public static string Unescape(this string s)
    {
        StringBuilder result = new();

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] != '\\' || i + 1 >= s.Length)
            {
                result.Append(s[i]);
                continue;
            }

            char next = s[++i];

            switch (next)
            {
                case '\\': result.Append('\\'); break;
                case '"': result.Append('"'); break;
                case 'n': result.Append('\n'); break;
                case 'r': result.Append('\r'); break;
                default:
                    result.Append('\\');
                    result.Append(next);
                    break;
            }
        }

        return result.ToString();
    }
    
    private static readonly Regex ToMacroCaseRegex = new("(?<!^)([A-Z][a-z]*)");
}