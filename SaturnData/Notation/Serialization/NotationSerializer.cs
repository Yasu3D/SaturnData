using System;
using System.Collections.Generic;
using System.IO;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization.Mer;
using SaturnData.Notation.Serialization.SatV1;
using SaturnData.Notation.Serialization.SatV2;
using SaturnData.Notation.Serialization.SatV3;
using SaturnData.Utilities;

namespace SaturnData.Notation.Serialization;

public static class NotationSerializer
{
    /// <summary>
    /// Converts a <see cref="Chart"/> into a string.
    /// </summary>
    /// <param name="chart">The <see cref="Chart"/> to serialize.</param>
    /// <remarks>
    /// This overload doesn't write any metadata. Certain format specs may not support this.
    /// </remarks>
    public static string ToString(Chart chart, NotationWriteArgs args)
    {
        return args.ChartFormatVersion switch
        {
            ChartFormatVersion.Mer => MerWriter.ToString(chart, args),
            ChartFormatVersion.SatV1 => SatV1Writer.ToString(chart, args),
            ChartFormatVersion.SatV2 => SatV2Writer.ToString(chart, args),
            ChartFormatVersion.SatV3 => SatV3Writer.ToString(chart, args),
            _ => "",
        };
    }
    
    /// <summary>
    /// Converts a <see cref="Chart"/> into a string.
    /// </summary>
    /// <param name="entry">The <see cref="Entry"/> to serialize.</param>
    /// <param name="chart">The <see cref="Chart"/> to serialize.</param>
    /// <param name="formatVersion">The format to serialize as.</param>
    public static string ToString(Entry? entry, Chart chart, NotationWriteArgs args)
    {
        return args.ChartFormatVersion switch
        {
            ChartFormatVersion.Mer => MerWriter.ToString(entry, chart, args),
            ChartFormatVersion.SatV1 => SatV1Writer.ToString(entry, chart, args),
            ChartFormatVersion.SatV2 => SatV2Writer.ToString(entry, chart, args),
            ChartFormatVersion.SatV3 => SatV3Writer.ToString(entry, chart, args),
            _ => "",
        };
    }

    /// <summary>
    /// Writes a chart to a file.
    /// </summary>
    public static void ToFile(string path, Entry entry, Chart chart, NotationWriteArgs args)
    {
        string data = ToString(entry, chart, args);
        File.WriteAllText(path, data);
    }

    /// <summary>
    /// Reads a file and converts it into a <see cref="Chart"/>.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <param name="args">Arguments for how the data should be read.</param>
    public static Chart ToChart(string path, NotationReadArgs args, out List<Exception> exceptions)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToChart(lines, args, out exceptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            exceptions = [ex];
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into a <see cref="Chart"/>.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <param name="args">Arguments for how the data should be read.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationReadArgs args, out List<Exception> exceptions)
    {
        try
        {
            ChartFormatVersion chartFormatVersion = DetectFormatVersion(lines);
            Chart chart = chartFormatVersion switch
            {
                ChartFormatVersion.Mer => MerReader.ToChart(lines, args, out exceptions),
                ChartFormatVersion.SatV1 => SatV1Reader.ToChart(lines, args, out exceptions),
                ChartFormatVersion.SatV2 => SatV2Reader.ToChart(lines, args, out exceptions),
                ChartFormatVersion.SatV3 => SatV3Reader.ToChart(lines, args, out exceptions),
                _ => throw new(),
            };
            
            return chart;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            exceptions = [ex];
            return new();
        }
    }

    /// <summary>
    /// Reads a file and converts it into an <see cref="Entry"/>.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <param name="args">Arguments for how the data should be read.</param>
    /// <returns></returns>
    public static Entry ToEntry(string path, NotationReadArgs args, out List<Exception> exceptions)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToEntry(lines, args, out exceptions, path);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            exceptions = [ex];
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <param name="args">Arguments for how the data should be read.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines, NotationReadArgs args, out List<Exception> exceptions, string path = "")
    {
        try
        {
            ChartFormatVersion chartFormatVersion = DetectFormatVersion(lines);
            
            return chartFormatVersion switch
            {
                ChartFormatVersion.Mer => MerReader.ToEntry(lines, args, out exceptions, path),
                ChartFormatVersion.SatV1 => SatV1Reader.ToEntry(lines, args, out exceptions, path),
                ChartFormatVersion.SatV2 => SatV2Reader.ToEntry(lines, args, out exceptions, path),
                ChartFormatVersion.SatV3 => SatV3Reader.ToEntry(lines, args, out exceptions, path),
                _ => throw new(),
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            exceptions = [ex];
            return new();
        }
    }
    
    public static ChartFormatVersion DetectFormatVersion(string path)
    {
        bool isMer = path.EndsWith(".mer", StringComparison.OrdinalIgnoreCase);
        bool isSat = path.EndsWith(".sat", StringComparison.OrdinalIgnoreCase);
        bool isMap = path.EndsWith(".map", StringComparison.OrdinalIgnoreCase);
        bool isTxt = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

        if (!isMer && !isSat && !isMap && !isTxt)
        {
            return ChartFormatVersion.Unknown;
        }
        
        try
        {
            string[] lines = File.ReadAllLines(path);
            return DetectFormatVersion(lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ChartFormatVersion.Unknown;
        }
    }
    
    public static ChartFormatVersion DetectFormatVersion(string[] lines)
    {
        ChartFormatVersion version = ChartFormatVersion.Unknown;
        foreach (string line in lines)
        {
            if (SerializationHelpers.ContainsKey(line, "@SAT_VERSION ", out string value))
            {
                version = value switch
                {
                    "1" => ChartFormatVersion.SatV1,
                    "2" => ChartFormatVersion.SatV2,
                    "3" => ChartFormatVersion.SatV3,
                    _ => ChartFormatVersion.Unknown,
                };
                break;
            }

            if (line.StartsWith("#BODY"))
            {
                version = ChartFormatVersion.Mer;
                break;
            }
        }
        
        return version;
    }
}