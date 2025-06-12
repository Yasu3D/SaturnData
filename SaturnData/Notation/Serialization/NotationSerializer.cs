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
    public static string ToString(Entry entry, Chart chart, FormatVersion formatVersion, NotationSerializerOptions options)
    {
        return formatVersion switch
        {
            FormatVersion.Mer => MerWriter.ToString(entry, chart, options),
            FormatVersion.SatV1 => SatV1Writer.ToString(entry, chart, options),
            FormatVersion.SatV2 => SatV2Writer.ToString(entry, chart, options),
            FormatVersion.SatV3 => SatV3Writer.ToString(entry, chart, options),
            _ => "",
        };
    }

    /// <summary>
    /// Reads a file and converts it into a chart.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns></returns>
    public static Chart ToChart(string path, NotationSerializerOptions options)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            return ToChart(lines, options);
        }
        catch
        {
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationSerializerOptions options)
    {
        try
        {
            FormatVersion formatVersion = NotationUtils.DetectFormatVersion(lines);
            return formatVersion switch
            {
                FormatVersion.Mer => MerReader.ToChart(lines, options),
                FormatVersion.SatV1 => SatV1Reader.ToChart(lines, options),
                FormatVersion.SatV2 => SatV2Reader.ToChart(lines, options),
                FormatVersion.SatV3 => SatV3Reader.ToChart(lines, options),
                _ => throw new(),
            };
        }
        catch
        {
            return new();
        }
    }

    /// <summary>
    /// Reads a file and converts it into an entry.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns></returns>
    public static Entry ToEntry(string path)
    {
        try
        {
            string[] lines = File.ReadAllLines(path);
            Entry entry = ToEntry(lines);
            entry.ChartPath = path;
            
            return entry;
        }
        catch
        {
            return new();
        }
    }
    
    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines)
    {
        try
        {
            FormatVersion formatVersion = NotationUtils.DetectFormatVersion(lines);

            return formatVersion switch
            {
                FormatVersion.Mer => MerReader.ToEntry(lines),
                FormatVersion.SatV1 => SatV1Reader.ToEntry(lines),
                FormatVersion.SatV2 => SatV2Reader.ToEntry(lines),
                FormatVersion.SatV3 => SatV3Reader.ToEntry(lines),
                _ => throw new(),
            };
        }
        catch
        {
            return new();
        }
    }
}