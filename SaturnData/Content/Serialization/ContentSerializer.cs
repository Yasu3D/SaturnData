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
    public static string ToString(ContentItem contentItem)
    {
        return SatContentV1Writer.ToString(contentItem);
    }

    /// <summary>
    /// Writes a <see cref="ContentItem"/> to a file.
    /// </summary>
    public static void ToFile(string path, ContentItem contentItem)
    {
        string data = ToString(contentItem);
        File.WriteAllText(path, data);
    }
    
    /// <summary>
    /// Reads a file and converts it into a <see cref="ContentItem"/>.
    /// </summary>
    /// <param name="path">The file to open.</param>
    public static ContentItem? ToContentItem(string path)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToContentItem(lines);
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
    /// <returns></returns>
    public static ContentItem? ToContentItem(string[] lines)
    {
        try
        {
            ContentFormatVersion contentFormatVersion = DetectFormatVersion(lines);
            ContentItem? contentItem = contentFormatVersion switch
            {
                ContentFormatVersion.SatContentV1 => SatContentV1Reader.ToContentItem(lines),
                ContentFormatVersion.Unknown => null,
                _ => throw new(),
            };
            
            return contentItem;
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
        bool isSatContent = path.EndsWith(".satc", StringComparison.OrdinalIgnoreCase);
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
                    _ => ContentFormatVersion.Unknown,
                };
                break;
            }
        }
        
        return version;
    }
}