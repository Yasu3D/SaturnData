using System;
using System.IO;
using SaturnData.Content.Items;
using SaturnData.Content.Serialization.SatContentV1;
using SaturnData.Utilities;

namespace SaturnData.Content.Serialization;

public enum ContentFormatVersion
{
    /// <summary>
    /// An unknown, unrecognized, or broken format that can't be parsed.
    /// </summary>
    Unknown = -1,
    
    /// <summary>
    /// First Saturn Content format. See <see href="https://saturn.yasu3d.art/docs/#/satcontent_format_1">Saturn Docs</see>.
    /// </summary>
    SatContentV1 = 0,
    
    /// <summary>
    /// Second Saturn Content format. See <see href="https://saturn.yasu3d.art/docs/#/satcontent_format_2">Saturn Docs</see>.
    /// </summary>
    SatContentV2 = 1,
}

public static class ContentSerializer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="chart">The chart to serialize.</param>
    /// <remarks>
    /// This overload doesn't write any metadata. Certain format specs may not support this.
    /// </remarks>
    public static string ToString(ContentItem contentItem, ContentFormatVersion formatVersion = ContentFormatVersion.SatContentV2)
    {
        return formatVersion switch
        {
            ContentFormatVersion.SatContentV1 => SatContentV1Writer.ToString(contentItem),
            ContentFormatVersion.SatContentV2 => SatContentV2Writer.ToString(contentItem),
            _ => "",
        };
    }

    /// <summary>
    /// Writes a <see cref="ContentItem"/> to a file.
    /// </summary>
    public static void ToFile(string path, ContentItem contentItem, ContentFormatVersion formatVersion = ContentFormatVersion.SatContentV2)
    {
        string data = ToString(contentItem, formatVersion);
        File.WriteAllText(path, data);
    }
    
    /// <summary>
    /// Reads a file and converts it into a <see cref="ContentItem"/>.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <typeparam name="T">The type of content item to try to read.</typeparam>
    public static T? ToContentItem<T>(string path) where T : ContentItem, new()
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToContentItem<T>(lines);
        }
        catch (Exception ex)
        {
            // Don't throw.
            Console.WriteLine(ex);
        }
        
        return null;
    }
    
    /// <summary>
    /// Reads content data and converts it into a <see cref="ContentItem"/>.
    /// </summary>
    /// <param name="lines">Content file data separated into individual lines.</param>
    /// <typeparam name="T">The type of content item to try to read.</typeparam>
    public static T? ToContentItem<T>(string[] lines) where T : ContentItem, new()
    {
        try
        {
            ContentFormatVersion formatVersion = DetectFormatVersion(lines);

            return formatVersion switch
            {
                ContentFormatVersion.Unknown => null,
                ContentFormatVersion.SatContentV1 => SatContentV1Reader.ToContentItem(lines) as T,
                ContentFormatVersion.SatContentV2 => SatContentV2Reader.ToContentItem<T>(lines),
                _ => null,
            };
        }
        catch (Exception ex)
        {
            // Don't throw.
            Console.WriteLine(ex);
        }
        
        return null;
    }
    
    public static ContentFormatVersion DetectFormatVersion(string path)
    {
        bool isSatContent = path.EndsWith(".sct", StringComparison.OrdinalIgnoreCase);
        bool isTxt = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

        if (!isSatContent && !isTxt)
        {
            return ContentFormatVersion.Unknown;
        }
        
        try
        {
            string[] lines = File.ReadAllLines(path);
            return DetectFormatVersion(lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ContentFormatVersion.Unknown;
        }
    }
    
    public static ContentFormatVersion DetectFormatVersion(string[] lines)
    {
        ContentFormatVersion version = ContentFormatVersion.Unknown;
        foreach (string line in lines)
        {
            if (SerializationHelpers.ContainsKey(line, "@SCT_VERSION ", out string value))
            {
                version = value switch
                {
                    "1" => ContentFormatVersion.SatContentV1,
                    "2" => ContentFormatVersion.SatContentV2,
                    _ => ContentFormatVersion.Unknown,
                };
                break;
            }
        }
        
        return version;
    }

    public static string DetectType(string path)
    {
        bool isSatContent = path.EndsWith(".sct", StringComparison.OrdinalIgnoreCase);
        bool isTxt = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

        if (!isSatContent && !isTxt)
        {
            return "UNKNOWN";
        }
        
        try
        {
            string[] lines = File.ReadAllLines(path);
            return DetectType(lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return "UNKNOWN";
        }
    }

    public static string DetectType(string[] lines)
    {
        foreach (string line in lines)
        {
            if (SerializationHelpers.ContainsKey(line, "@TYPE ", out string value))
            {
                return value;
            }
        }
        
        return "UNKNOWN";
    }
}