using System;
using System.IO;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization.Mer;
using SaturnData.Notation.Serialization.SatV1;
using SaturnData.Notation.Serialization.SatV2;
using SaturnData.Notation.Serialization.SatV3;

namespace SaturnData.Notation.Serialization;

public static class NotationSerializer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <param name="formatVersion">The format to serialize as.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationWriteArgs args)
    {
        return args.FormatVersion switch
        {
            FormatVersion.Mer => MerWriter.ToString(entry, chart, args),
            FormatVersion.SatV1 => SatV1Writer.ToString(entry, chart, args),
            FormatVersion.SatV2 => SatV2Writer.ToString(entry, chart, args),
            FormatVersion.SatV3 => SatV3Writer.ToString(entry, chart, args),
            _ => "",
        };
    }

    public static void ToFile(string path, Entry entry, Chart chart, NotationWriteArgs args)
    {
        string data = ToString(entry, chart, args);
        File.WriteAllText(path, data);
    }

    /// <summary>
    /// Reads a file and converts it into a chart.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns></returns>
    public static Chart ToChart(string path, NotationReadArgs args)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToChart(lines, args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationReadArgs args)
    {
        try
        {
            FormatVersion formatVersion = DetectFormatVersion(lines);
            return formatVersion switch
            {
                FormatVersion.Mer => MerReader.ToChart(lines, args),
                FormatVersion.SatV1 => SatV1Reader.ToChart(lines, args),
                FormatVersion.SatV2 => SatV2Reader.ToChart(lines, args),
                FormatVersion.SatV3 => SatV3Reader.ToChart(lines, args),
                _ => throw new(),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }

    /// <summary>
    /// Reads a file and converts it into an entry.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns></returns>
    public static Entry ToEntry(string path, NotationReadArgs args)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            Entry entry = ToEntry(lines, args);
            entry.ChartPath = path;
            
            return entry;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines, NotationReadArgs args)
    {
        try
        {
            FormatVersion formatVersion = DetectFormatVersion(lines);

            return formatVersion switch
            {
                FormatVersion.Mer => MerReader.ToEntry(lines),
                FormatVersion.SatV1 => SatV1Reader.ToEntry(lines, args),
                FormatVersion.SatV2 => SatV2Reader.ToEntry(lines, args),
                FormatVersion.SatV3 => SatV3Reader.ToEntry(lines, args),
                _ => throw new(),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new();
        }
    }
    
    public static FormatVersion DetectFormatVersion(string path)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return DetectFormatVersion(lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return FormatVersion.Unknown;
        }
    }
    
    public static FormatVersion DetectFormatVersion(string[] lines)
    {
        FormatVersion version = FormatVersion.Unknown;
        foreach (string line in lines)
        {
            if (NotationUtils.ContainsKey(line, "@SAT_VERSION ", out string value))
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

            if (line.StartsWith("#BODY"))
            {
                version = FormatVersion.Mer;
                break;
            }
        }
        
        return version;
    }
}