using System;
using System.Collections.Generic;
using SaturnData.Notation.Core;

namespace SaturnData.Notation.Serialization;

internal static class NotationUtils
{
    internal static void AddOrCreate<T>(Dictionary<int, Layer<T>> dictionary, int key, T value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, new($"Layer {key}"));
        }
        
        dictionary[key].Items.Add(value);
    }
    
    internal static bool ContainsKey(string input, string key, out string value)
    {
        if (input.Contains(key))
        {
            value = input[(input.IndexOf(key, StringComparison.Ordinal) + key.Length)..].Trim();
            return true;
        }

        value = "";
        return false;
    }
    
    internal static FormatVersion DetectFormatVersion(string[] lines)
    {
        FormatVersion version = FormatVersion.Unknown;
        foreach (string line in lines)
        {
            if (ContainsKey(line, "@SAT_VERSION ", out string value))
            {
                version = value switch
                {
                    "1" => FormatVersion.SatV1,
                    "2" => FormatVersion.SatV2,
                    "3" => FormatVersion.SatV3,
                    _ => FormatVersion.Unknown,
                };
                break;
            }

            if (line.Contains("#BODY"))
            {
                version = FormatVersion.Mer;
                break;
            }
        }
        
        return version;
    }
}