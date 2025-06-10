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
            FormatVersion.Mer => MerSerializer.ToString(entry, chart, options),
            FormatVersion.SatV1 => SatV1Serializer.ToString(entry, chart),
            FormatVersion.SatV2 => SatV2Serializer.ToString(entry, chart),
            FormatVersion.SatV3 => SatV3Serializer.ToString(entry, chart),
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
                FormatVersion.Mer => MerSerializer.ToChart(lines, options),
                FormatVersion.SatV1 => SatV1Serializer.ToChart(lines),
                FormatVersion.SatV2 => SatV2Serializer.ToChart(lines),
                FormatVersion.SatV3 => SatV3Serializer.ToChart(lines),
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
                FormatVersion.Mer => MerSerializer.ToEntry(lines),
                FormatVersion.SatV1 => SatV1Serializer.ToEntry(lines),
                FormatVersion.SatV2 => SatV2Serializer.ToEntry(lines),
                FormatVersion.SatV3 => SatV3Serializer.ToEntry(lines),
                _ => throw new(),
            };
        }
        catch
        {
            return new();
        }
    }
}