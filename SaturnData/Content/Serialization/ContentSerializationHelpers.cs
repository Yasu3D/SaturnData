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

    public const string Indent = "    ";
    
    private static readonly Regex ToMacroCaseRegex = new("(?<!^)([A-Z][a-z]*)");
}