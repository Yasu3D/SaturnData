using System;

namespace SaturnData.Utilities;

public static class SerializationHelpers
{
    public static bool ContainsKey(string input, string key, out string value)
    {
        if (input.StartsWith(key))
        {
            value = input[(input.IndexOf(key, StringComparison.Ordinal) + key.Length)..].Trim();
            return true;
        }

        value = "";
        return false;
    }
}